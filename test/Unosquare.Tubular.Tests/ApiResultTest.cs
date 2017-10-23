namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using Unosquare.Tubular.GenericModels;

    [TestFixture]
    class ApiResultTest
    {
        [Test]
        public void AffectedCountTests()
        {
            var api = new ApiResult();
            var affectedCount = 1000;

            api.AffectedCount = affectedCount;

            Assert.AreEqual(affectedCount, api.AffectedCount);
        }

        [Test]
        public void OKTes()
        {
            var apiOK = new ApiResult();
            var message = "This is a OK message";

            var result = ApiResult.Ok(message);

            Assert.AreEqual("OK", result.Status);
            Assert.AreEqual(message, result.Message);
        }

        [Test]
        public void ErrorTes()
        {
            var apiOK = new ApiResult();
            var ex = new System.Exception("This is a Error message");

            var result = ApiResult.Error(ex);

            Assert.AreEqual("Error", result.Status);
            Assert.AreEqual(ex.Message, result.Message);
        }
    }
}
