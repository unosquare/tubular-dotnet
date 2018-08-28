namespace Unosquare.Tubular.Tests
{
    using Database;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class ChartFixture
    {
        private const string ColorTest = "red";

        [Test]
        public void GetSingleSerieChart()
        {
            var sut = SampleEntities.GenerateData().ToList();

            var chartObj = sut.AsQueryable()
                .ProvideSingleSerieChartResponse(
                    label: x => x.Color,
                    value: x => x.DecimalNumber);

            var query = sut.Where(x => x.Color == ColorTest)
                .Sum(x => x.DecimalNumber);

            Assert.IsNotNull(chartObj);

            var colorIndex = chartObj.Labels.ToList().IndexOf(ColorTest);

            Assert.IsTrue(colorIndex >= 0);

            var value = chartObj.Data[colorIndex];

            Assert.AreEqual(value, query);
        }
    }
}