namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using GenericModels;

    [TestFixture]
    public class ApiResultTest
    {
        [Test]
        public void AffectedCountTests()
        {
            var api = new ApiResult();
            const int affectedCount = 1000;

            api.AffectedCount = affectedCount;

            Assert.AreEqual(affectedCount, api.AffectedCount);
        }

        [Test]
        public void Okest()
        {
            const string message = "This is a OK message";

            var result = ApiResult.Ok(message);

            Assert.AreEqual("Ok", result.Status);
            Assert.AreEqual(message, result.Message);
        }

        [Test]
        public void ErrorTest()
        {
            var ex = new System.Exception("This is a Error message");

            var result = ApiResult.Error(ex);

            Assert.AreEqual("Error", result.Status);
            Assert.AreEqual(ex.Message, result.Message);
        }
    }
}