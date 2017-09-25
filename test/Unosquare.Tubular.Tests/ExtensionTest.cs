namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using Unosquare.Tubular.ObjectModel;
    using Unosquare.Tubular.Tests.Database;
    using static Unosquare.Tubular.Extensions;

    [TestFixture]
    class ExtensionTest
    {
        private const int PageSize = 20;

        [Test]
        public void CreateGridDataResponseThrowArgumentExceptionTest()
        {
            var filters = new[] { "blue", "red" };
            var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.Equals(filters[0]) && x.Color.Equals(filters[1]));
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest()
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = null
            };

            Assert.Throws<ArgumentNullException>(() =>
            {
                var response = request.CreateGridDataResponse(dataSource);
            });
        }
    }
}
