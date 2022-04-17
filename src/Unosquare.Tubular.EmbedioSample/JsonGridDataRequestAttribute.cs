using EmbedIO;
using EmbedIO.WebApi;

namespace Unosquare.Tubular.EmbedioSample;

[AttributeUsage(AttributeTargets.Parameter)]
public class JsonGridDataRequestAttribute : Attribute, IRequestDataAttribute<WebApiController, GridDataRequest>
{
    public Task<GridDataRequest> GetRequestDataAsync(WebApiController controller, string parameterName)
        => controller.HttpContext.GetRequestDataAsync(RequestDeserializer.Json<GridDataRequest>);
}