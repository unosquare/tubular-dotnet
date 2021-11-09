using EmbedIO;
using EmbedIO.WebApi;
using System;
using System.Threading.Tasks;

namespace Unosquare.Tubular.EmbedioSample
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonGridDataRequestAttribute : Attribute, IRequestDataAttribute<WebApiController, GridDataRequest>
    {
        public Task<GridDataRequest> GetRequestDataAsync(WebApiController controller, string parameterName)
            => controller.HttpContext.GetRequestDataAsync(RequestDeserializer.Json<GridDataRequest>);
    }
}