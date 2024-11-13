using Unosquare.Tubular.Tests.Database;
using NUnit.Framework;

namespace Unosquare.Tubular.Tests;

[TestFixture]
public class ExtensionTest
{
    private const int PageSize = 20;

    [Test]
    public void CreateGridDataResponseThrowArgumentExceptionTest()
    {
        var filters = new[] { "blue", "red" };
        var dataSource = SampleEntities.GenerateData().AsQueryable().Where(x => x.Color.Equals(filters[0]) && x.Color.Equals(filters[1]));

        var request = new GridDataRequest
        {
            Take = PageSize,
            Skip = 0,
            Columns = null,
        };

        Assert.Throws<ArgumentNullException>(() => request.CreateGridDataResponse(dataSource));
    }
}