namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using Unosquare.Tubular.ObjectModel;
    using Unosquare.Tubular.Tests.Database;

    [TestFixture]
    class CompareOperatorsTest
    {
        private const int PageSize = 20;
        private readonly IQueryable<Thing> dataSource = SampleEntities.GenerateData().AsQueryable();

        [Test]
        public void EqualsFilterTest()
        {
            var filter = "blue";
            var filterCount = dataSource.Where(x => x.Color.Equals(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void BetweenFilterTest()
        {
            var a = 10;
            var b = new string[] { "30" };
            var filterCount = dataSource.Where(x => x.Id >= a && x.Id <= int.Parse(b[0]));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithBetweenFilter(a.ToString(), b)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void ContainsFilterTest()
        {
            var filter = "l";
            var filterCount = dataSource.Where(x => x.Color.Contains(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.Contains)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void EndsWithFilterTest()
        {
            var filter = "ow";
            var filterCount = dataSource.Where(x => x.Color.EndsWith(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.EndsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void GtFilterTest()
        {
            var filter = 20;
            var filterCount = dataSource.Where(x => x.Id > filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter(filter.ToString(), CompareOperators.Gt)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void GteFilterTest()
        {
            var filter = 20;
            var filterCount = dataSource.Where(x => x.Id >= filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter(filter.ToString(), CompareOperators.Gte)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void LtFilterTest()
        {
            var filter = 20;
            var filterCount = dataSource.Where(x => x.Id < filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter(filter.ToString(), CompareOperators.Lt)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void LteFilterTest()
        {
            var filter = 20;
            var filterCount = dataSource.Where(x => x.Id <= filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter(filter.ToString(), CompareOperators.Lte)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotContainsFilterTest()
        {
            var filter = "l";
            var filterCount = dataSource.Where(x => !x.Color.Contains(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotContains)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotEndsWithFilterTest()
        {
            var filter = "ow";
            var filterCount = dataSource.Where(x => !x.Color.EndsWith(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotEndsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotEqualsFilterTest()
        {
            var filter = "blue";
            var filterCount = dataSource.Where(x => !x.Color.Equals(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotEquals)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotStartsWithFilterTest()
        {
            var filter = "blue";
            var filterCount = dataSource.Where(x => !x.Color.StartsWith(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotStartsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void StartsWithFilterTest()
        {
            var filter = "blue";
            var filterCount = dataSource.Where(x => x.Color.StartsWith(filter));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.StartsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NoneFilterTest()
        {
            var filter = string.Empty;
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.None)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void AutoFilterTest()
        {
            var filter = string.Empty;
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.Auto)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void MultipleFilterTest()
        {
            var filters = new[] { "blue", "red" };
            var filterCount = dataSource.Where(x => x.Color.Equals(filters[0]) || x.Color.Equals(filters[1]));
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithMultipleFilter(filters, CompareOperators.Multiple)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void DateEqualFilterTest()
        {
            var filter = DateTime.UtcNow.Date.ToString();
            Assert.IsTrue(false, "OHHH" + filter + " - " + dataSource.FirstOrDefault()?.Date.Date.ToString());

            var filterCount = dataSource.Where(x => x.Date.Date.ToString() == filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithDateFilter(filter, CompareOperators.Equals, Tubular.DataType.Date)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void DecimalNumberFilterTest()
        {
            var filter = 10.100m;
            var filterCount = dataSource.Where(x => x.DecimalNumber == filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithNumberFilter(filter.ToString(), CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }
    }
}
