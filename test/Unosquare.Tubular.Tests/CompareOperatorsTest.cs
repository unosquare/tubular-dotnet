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

        static object[] FilterCases =
        {
            new object[] { "EqualsFilterTest", _dataSource.Where(x => x.Color.Equals("blue")),  Thing.GetColumnsWithColorFilter("blue", CompareOperators.Equals)},
            new object[] { "ContainsFilterTest", _dataSource.Where(x => x.Color.Contains("l")), Thing.GetColumnsWithColorFilter("l", CompareOperators.Contains) },
            new object[] { "EndsWithFilterTest", _dataSource.Where(x => x.Color.EndsWith("ow")), Thing.GetColumnsWithColorFilter("ow", CompareOperators.EndsWith) },
            new object[] { "BetweenFilterTest",  _dataSource.Where(x => x.Id >= 10 && x.Id <= 30), Thing.GetColumnsWithBetweenFilter("10", new[] { "30"}) },
            new object[] { "GtFilterTest", _dataSource.Where(x => x.Id > 20), Thing.GetColumnsWithIdFilter("20", CompareOperators.Gt) },
            new object[] { "GteFilterTest", _dataSource.Where(x => x.Id >= 20), Thing.GetColumnsWithIdFilter("20", CompareOperators.Gte) },
            new object[] { "LtFilterTest", _dataSource.Where(x => x.Id < 20), Thing.GetColumnsWithIdFilter("20", CompareOperators.Lt) },
            new object[] { "LteFilterTest", _dataSource.Where(x => x.Id <= 20), Thing.GetColumnsWithIdFilter("20", CompareOperators.Lte) },
            new object[] { "NotContainsFilterTest", _dataSource.Where(x => !x.Color.Contains("l")), Thing.GetColumnsWithColorFilter("l", CompareOperators.NotContains) },
            new object[] { "NotEndsWithFilterTest", _dataSource.Where(x => !x.Color.EndsWith("ow")), Thing.GetColumnsWithColorFilter("ow", CompareOperators.NotEndsWith) },
            new object[] { "NotEqualsFilterTest", _dataSource.Where(x => !x.Color.Equals("blue")), Thing.GetColumnsWithColorFilter("blue", CompareOperators.NotEquals) },
            new object[] { "NotStartsWithFilterTest", _dataSource.Where(x => !x.Color.StartsWith("yell")), Thing.GetColumnsWithColorFilter("yell", CompareOperators.NotStartsWith) },
            new object[] { "StartsWithFilterTest", _dataSource.Where(x => x.Color.StartsWith("yell")), Thing.GetColumnsWithColorFilter("yell", CompareOperators.StartsWith) },
            new object[] { "NoneFilterTest", _dataSource, Thing.GetColumnsWithColorFilter(string.Empty, CompareOperators.None) },
            new object[] { "AutoFilterTest", _dataSource, Thing.GetColumnsWithColorFilter(string.Empty, CompareOperators.Auto) },
            new object[] { "MultipleFilterTest", _dataSource.Where(x => x.Color.Equals("blue") || x.Color.Equals("red")), Thing.GetColumnsWithMultipleFilter(new[] { "blue", "red" }, CompareOperators.Multiple) },
            new object[] { "DateEqualFilterTest", _dataSource.Where(x => x.Date.Date.ToString() == DateTime.Now.Date.ToString()), Thing.GetColumnsWithDateFilter(DateTime.Now.Date.ToString(), CompareOperators.Equals, DataType.Date) },
            new object[] { "DecimalNumberFilterTest", _dataSource.Where(x => x.DecimalNumber == 10.100m), Thing.GetColumnsWithNumberFilter(10.100m.ToString(), CompareOperators.Equals) },
        };

        [Test, TestCaseSource("FilterCases")]
        public void FiltersTests(string test, IQueryable<Thing> filterCount, GridColumn[] columns)
        {
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = columns
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, test);

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }
    }
}