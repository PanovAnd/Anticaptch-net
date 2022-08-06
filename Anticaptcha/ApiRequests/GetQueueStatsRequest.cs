using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Anticaptcha.ApiRequests{
    public enum QueueType{
        ImageToTextEnglish = 1,
        ImageToTextRussian = 2,
        Recaptchav2 = 5,
        Recaptchav2Proxyless = 6,
        Funcaptcha = 7,
        FuncaptchaProxyless = 10,
        GeeTest = 12,
        GeeTestProxyless = 13,
        RecaptchaV3MinScore03 = 18,
        RecaptchaV3MinScore07 = 19,
        RecaptchaV3MinScore09 = 20,
        HCaptcha = 21,
        HCaptchaProxyless = 22,
        RecaptchaEnterpriseV2 = 23,
        RecaptchaEnterpriseV2Proxyless = 24,
        AntiGateTask = 25
    }

    internal class GetQueueStatsRequest : IApiRequest{
        [JsonProperty("queueId")]
        internal readonly QueueType QueueType;

        internal GetQueueStatsRequest(QueueType queueType){
            QueueType = queueType;
        }

        private const string _anticaptchaEndpoint = "https://api.anti-captcha.com/getQueueStats";

        public HttpRequestMessage CreateHttpRequest(){
            var request = new HttpRequestMessage(HttpMethod.Post, _anticaptchaEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return request;
        }
    }

    public class GetQueueStatsResponse : ErrorResponse{
        [JsonProperty("waiting")]
        public int Waiting{ get; private set; }

        [JsonProperty("load")]
        public double Load{ get; private set; }

        [JsonProperty("bid")]
        public double Bid{ get; private set; }

        [JsonProperty("speed")]
        public double Speed{ get; private set; }

        [JsonProperty("total")]
        public int Total{ get; private set; }
    }
}