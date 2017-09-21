namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System.Linq;
    using Unosquare.Tubular.ObjectModel;
    using Unosquare.Tubular.Tests.Database;

    [TestFixture]
    class CompareOperatorsTest
    {
        private const int PageSize = 20;

        [Test]
        public void EqualsFilterTest()
        {
            var filter = "blue";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.Equals(filter));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void BetwenFilterTest()
        {
            var a = 10;
            var b = 30;
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Id >= a && x.Id <= b);
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithBetweenFilter(a.ToString(), b.ToString())
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(data[0].Id, response.Payload[0][0], "Same Id");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void ContainsFilterTest()
        {
            var filter = "l";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.Contains(filter));
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void EndsWithFilterTest()
        {
            var filter = "ow";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.EndsWith(filter));
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void GtFilterTest()
        {
            var filter = 20;
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Id > filter);
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void GteFilterTest()
        {
            var filter = 20;
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Id >= filter);
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void LtFilterTest()
        {
            var filter = 20;
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Id < filter);
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void LteFilterTest()
        {
            var filter = 20;
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Id <= filter);
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotContainsFilterTest()
        {
            var filter = "l";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => !x.Color.Contains(filter));
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotEndsWithFilterTest()
        {
            var filter = "ow";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => !x.Color.EndsWith(filter));
            var data = dataSource.Take(PageSize).ToList();

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

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotEqualsFilterTest()
        {
            var filter = "blue";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => !x.Color.Equals(filter));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotEquals)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NotStartsWithFilterTest()
        {
            var filter = "blue";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => !x.Color.StartsWith(filter));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.NotStartsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void StartsWithFilterTest()
        {
            var filter = "blue";
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.StartsWith(filter));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.StartsWith)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void NoneFilterTest()
        {
            var filter = string.Empty;
            var dataSource = SampleEntities.GenerateData().AsQueryable();
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
            var dataSource = SampleEntities.GenerateData().AsQueryable();
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
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.Equals(filters[0]) && x.Color.Equals(filters[1]));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithMultipleFilter(filters, CompareOperators.Multiple)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }
    }
}
