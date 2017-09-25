namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System.Linq;
    using Unosquare.Tubular.ObjectModel;
    using Unosquare.Tubular.Tests.Database;

    [TestFixture]
    class AggregationTest
    {
        private const int PageSize = 20;
        private readonly IQueryable<Thing> dataSource = SampleEntities.GenerateData().AsQueryable();

        [Test]
        public void SumAggregationTest()
        {
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Sum)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Sum(x => x.Number), response.AggregationPayload["Number"],
               "Same sum number");

        }

        [Test]
        public void AverageAggregationTest()
        {
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Average)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Sum(x => x.Number)/dataSource.Count(), response.AggregationPayload["Number"],
               "Same average number");

        }

        [Test]
        public void MaxAggregationTest()
        {
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregateInt(AggregationFunction.Max)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Max(x => x.Id), response.AggregationPayload["Id"],
               "Same max number");
        }

        [Test]
        public void MinAggregationTest()
        {
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregateInt(AggregationFunction.Min)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Min(x => x.Id), response.AggregationPayload["Id"],
               "Same min number");
        }

        [Test]
        public void CountAggregationTest()
        {
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Count)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(dataSource.Count(), response.AggregationPayload["Number"],
               "Same count number");
        }
    }
}
