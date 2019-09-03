namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System;
    using System.Globalization;
    using System.Linq;
    using Database;

    [TestFixture]
    public class CompareOperatorsTest
    {
        private const int PageSize = 20;

        private static readonly IQueryable<Thing> DataSource = SampleEntities.GenerateData().ToList().AsQueryable();

        private static readonly object[] FilterColorCases =
        {
            // Filter, filterCount, Operator 
            new object[] {"blue", DataSource.Where(x => x.Color.Equals("blue")), CompareOperators.Equals},
            new object[] {"l", DataSource.Where(x => x.Color.Contains("l")), CompareOperators.Contains},
            new object[] {"ow", DataSource.Where(x => x.Color.EndsWith("ow")), CompareOperators.EndsWith},
            new object[] {"l", DataSource.Where(x => !x.Color.Contains("l")), CompareOperators.NotContains},
            new object[] {"ow", DataSource.Where(x => !x.Color.EndsWith("ow")), CompareOperators.NotEndsWith},
            new object[] {"blue", DataSource.Where(x => !x.Color.Equals("blue")), CompareOperators.NotEquals},
            new object[] {"yell", DataSource.Where(x => !x.Color.StartsWith("yell")), CompareOperators.NotStartsWith},
            new object[] {"yell", DataSource.Where(x => x.Color.StartsWith("yell")), CompareOperators.StartsWith},
            new object[] {string.Empty, DataSource, CompareOperators.None},
            new object[] {string.Empty, DataSource, CompareOperators.Auto}
        };

        private static readonly object[] FilterIdCases =
        {
            // filterCount, Operator 
            new object[] {DataSource.Where(x => x.Id > 20), CompareOperators.Gt},
            new object[] {DataSource.Where(x => x.Id >= 20), CompareOperators.Gte},
            new object[] {DataSource.Where(x => x.Id < 20), CompareOperators.Lt},
            new object[] {DataSource.Where(x => x.Id <= 20), CompareOperators.Lte},
        };

        [Test, TestCaseSource(nameof(FilterColorCases))]
        public void FilterColorTests(string filter, IQueryable<Thing> filterCount, CompareOperators compareOperator)
        {
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, compareOperator)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, $"Test {compareOperator}");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test, TestCaseSource(nameof(FilterIdCases))]
        public void FilterIdTests(IQueryable<Thing> filterCount, CompareOperators compareOperator)
        {
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter("20", compareOperator)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, $"Test {compareOperator}");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void BetweenFilterTest()
        {
            const int a = 10;
            var b = new[] { "30" };
            var filterCount = DataSource.Where(x => x.Id >= a && x.Id <= int.Parse(b[0]));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithBetweenFilter(a.ToString(), b)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void DecimalNumberFilterTest()
        {
            const decimal filter = 1.101m;
            var filterCount = DataSource.Where(x => x.DecimalNumber == filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithNumberFilter(filter.ToString(), CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void DateTimeEqualFilterTest()
        {
            var filter = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);

            var filterCount = DataSource.Where(x => x.Date.ToString(CultureInfo.InvariantCulture) == filter);

            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithDateFilter(filter, CompareOperators.Equals, DataType.DateTime)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void MultipleFilterTest()
        {
            var filters = new[] { "blue", "red" };
            var sut = DataSource.ToList();
            var filterCount = sut.Where(x => x.Color.Equals(filters[0]) || x.Color.Equals(filters[1]));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithMultipleFilter(filters, CompareOperators.Multiple)
            };

            var response = request.CreateGridDataResponse(sut.AsQueryable());

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void BooleanFilterTests()
        {
            const string filter = "true";
            var filterCount = DataSource.Where(x => x.IsShipped == bool.Parse(filter));

            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithBooleanFilter(filter, CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(DataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }
    }
}