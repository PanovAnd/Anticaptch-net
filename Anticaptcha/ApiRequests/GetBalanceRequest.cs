using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    internal class GetBalanceRequest:AuthorizedRequest,IApiRequest{
        internal GetBalanceRequest(string apiKey) : base(new AuthorizeInfo(apiKey, null)) { }
        
        private const string _anticaptchaEndpoint = @"https://api.anti-captcha.com/getBalance";
        public HttpRequestMessage CreateHttpRequest(){
            var request = new HttpRequestMessage(HttpMethod.Post, _anticaptchaEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return request;
        }
    }

    public class GetBalanceResponse : ErrorResponse{
        [JsonProperty("balance")]
        public double Balance;
        
    }
    
}