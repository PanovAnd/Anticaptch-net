using System.Net.Http;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    
    internal abstract class AuthorizedRequest {
        [JsonProperty("clientKey")]
        public readonly string ApiKey;

        [JsonProperty(PropertyName = "softId", NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? SoftId;

        public AuthorizedRequest(AuthorizeInfo authorizeInfo) {
            ApiKey = authorizeInfo.ApiKey;
            SoftId = authorizeInfo.SoftId;
        }

    }

    public interface IApiRequest{
        HttpRequestMessage CreateHttpRequest();
    }
}