namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using ObjectModel;
    using Database;

    [TestFixture]
    class CompareOperatorsTest
    {
        private const int PageSize = 20;
        private static readonly IQueryable<Thing> _dataSource = SampleEntities.GenerateData().AsQueryable();

        static object[] FilterColorCases =
        {
            // Filter, filterCount, Operator 
            new object[] { "blue", _dataSource.Where(x => x.Color.Equals("blue")), CompareOperators.Equals },
            new object[] { "l", _dataSource.Where(x => x.Color.Contains("l")), CompareOperators.Contains },
            new object[] { "ow", _dataSource.Where(x => x.Color.EndsWith("ow")), CompareOperators.EndsWith },
            new object[] { "l", _dataSource.Where(x => !x.Color.Contains("l")), CompareOperators.NotContains },
            new object[] { "ow", _dataSource.Where(x => !x.Color.EndsWith("ow")), CompareOperators.NotEndsWith },
            new object[] { "blue", _dataSource.Where(x => !x.Color.Equals("blue")), CompareOperators.NotEquals },
            new object[] { "yell", _dataSource.Where(x => !x.Color.StartsWith("yell")), CompareOperators.NotStartsWith },
            new object[] { "yell", _dataSource.Where(x => x.Color.StartsWith("yell")), CompareOperators.StartsWith },
            new object[] { string.Empty, _dataSource, CompareOperators.None },
            new object[] { string.Empty, _dataSource, CompareOperators.Auto }
        };

        static object[] FilterIdCases =
        {
            // filterCount, Operator 
            new object[] { _dataSource.Where(x => x.Id > 20), CompareOperators.Gt },
            new object[] { _dataSource.Where(x => x.Id >= 20), CompareOperators.Gte },
            new object[] { _dataSource.Where(x => x.Id < 20), CompareOperators.Lt },
            new object[] { _dataSource.Where(x => x.Id <= 20), CompareOperators.Lte },
        };

        [Test, TestCaseSource("FilterColorCases")]
        public void FilterColorTests(string filter, IQueryable<Thing> filterCount, CompareOperators compareOperator)
        {
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, compareOperator)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, $"Test {compareOperator}");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test, TestCaseSource("FilterIdCases")]
        public void FilterIdTests(IQueryable<Thing> filterCount, CompareOperators compareOperator)
        {
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter("20", compareOperator)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, $"Test {compareOperator}");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]		
         public void BetweenFilterTest()
         {		
             var a = 10;		
             var b = new[] { "30" };		
             var filterCount = _dataSource.Where(x => x.Id >= a && x.Id <= int.Parse(b[0]));		
             var data = filterCount.Take(PageSize).ToList();		
 		
             var request = new GridDataRequest()
             {		
                 Take = PageSize,		
                 Skip = 0,		
                 Search = new Filter(),		
                 Columns = Thing.GetColumnsWithBetweenFilter(a.ToString(), b)		
             };		
 		
             var response = request.CreateGridDataResponse(_dataSource);		
 		
             Assert.AreEqual(data.Count, response.Payload.Count, "Same length");		
 		
             Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");		
 		
             Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");		
         }

        [Test]		
         public void DecimalNumberFilterTest()
         {		
             var filter = 10.100m;		
             var filterCount = _dataSource.Where(x => x.DecimalNumber == filter);		
             var data = filterCount.Take(PageSize).ToList();		
 		
             var request = new GridDataRequest()
             {		
                 Take = PageSize,		
                 Skip = 0,		
                 Search = new Filter(),		
                 Columns = Thing.GetColumnsWithNumberFilter(filter.ToString(), CompareOperators.Equals)		
             };		
 		
             var response = request.CreateGridDataResponse(_dataSource);		
 		
             Assert.AreEqual(data.Count, response.Payload.Count, "Same length");		
 		
             Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");		
         }

        [Test]		
         public void DateEqualFilterTest()
         {
            var filter = DateTime.Now;
 		
             var filterCount = _dataSource.Where(x => x.Date.Date.ToString() == filter.Date.ToString());		
             var data = filterCount.Take(PageSize).ToList();		
 		
             var request = new GridDataRequest()
             {		
                 Take = PageSize,		
                 Skip = 0,		
                 Search = new Filter(),		
                 Columns = Thing.GetColumnsWithDateFilter(filter.ToString(), CompareOperators.Equals, DataType.Date)		
             };		
 		
             var response = request.CreateGridDataResponse(_dataSource);		
 		
             Assert.AreEqual(data.Count, response.Payload.Count, "Response date: " +		
                                                                 response.Payload.FirstOrDefault()?[3] +		
                                                                 "Filter date: " + filterCount.FirstOrDefault()?.Date);		
 		
             Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
         }

        [Test]
        public void DateTimeUTCEqualFilterTest()
        {
            var filter = DateTime.UtcNow.ToString();

            var filterCount = _dataSource.Where(x => x.Date.ToString() == filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithDateFilter(filter, CompareOperators.Equals, DataType.DateTimeUtc)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Response date: " +
                                                                response.Payload.FirstOrDefault()?[3] +
                                                                "Filter date: " + filterCount.FirstOrDefault()?.Date);

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]		
         public void MultipleFilterTest()
         {		
             var filters = new[] { "blue", "red" };		
             var filterCount = _dataSource.Where(x => x.Color.Equals(filters[0]) || x.Color.Equals(filters[1]));		
             var data = filterCount.Take(PageSize).ToList();		
 		
             var request = new GridDataRequest()
             {		
                 Take = PageSize,		
                 Skip = 0,		
                 Search = new Filter(),		
                 Columns = Thing.GetColumnsWithMultipleFilter(filters, CompareOperators.Multiple)		
             };		
 		
             var response = request.CreateGridDataResponse(_dataSource);		
 		
             Assert.AreEqual(data.Count, response.Payload.Count, "Same length");		
 		
             Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");		
         }

        [Test]
        public void BooleanFilterTests()
        {
            var filter = "true";
            var filterCount = _dataSource.Where(x => x.Bool == bool.Parse(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithBooleanFilter(filter, CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");

        }
    }
}