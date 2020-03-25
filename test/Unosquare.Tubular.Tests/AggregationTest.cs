namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System.Linq;
    using Database;

    [TestFixture]
    public class AggregationTest
    {
        private const int PageSize = 20;
        private readonly IQueryable<Thing> _dataSource = SampleEntities.GenerateData().AsQueryable();

        [Test]
        public void SumAggregationTest()
        {
            var data = _dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Sum)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(_dataSource.Sum(x => x.Number), response.AggregationPayload["Number"],
                "Same sum number");
        }

        [Test]
        public void AverageAggregationTest()
        {
            var data = _dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,

                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Average)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(_dataSource.Sum(x => x.Number) / _dataSource.Count(), response.AggregationPayload["Number"],
                "Same average number");

        }

        [Test]
        public void MaxAggregationTest()
        {
            var data = _dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Columns = Thing.GetColumnsWithAggregateInt(AggregationFunction.Max)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(_dataSource.Max(x => x.Id), response.AggregationPayload["Id"],
                "Same max number");
        }

        [Test]
        public void MinAggregationTest()
        {
            var data = _dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Columns = Thing.GetColumnsWithAggregateInt(AggregationFunction.Min)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(_dataSource.Min(x => x.Id), response.AggregationPayload["Id"],
                "Same min number");
        }

        [Test]
        public void CountAggregationTest()
        {
            var data = _dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Columns = Thing.GetColumnsWithAggregateDouble(AggregationFunction.Count)
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(_dataSource.Count(), response.AggregationPayload["Number"],
                "Same count number");
        }

        [Test]
        public void EmptySetSumAggregation_ReturnsSumZero()
        {
            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Columns = Thing.GetColumnsWithAggregateDoubleAndInvalidDate(AggregationFunction.Sum),
                TimezoneOffset = 360
            };

            var response = request.CreateGridDataResponse(_dataSource);

            Assert.AreEqual(0, response.Payload.Count, "Same length");
            Assert.IsTrue(response.AggregationPayload.Any());
            Assert.AreEqual(0, response.AggregationPayload.First().Value, "Sum zero");
        }
    }
}