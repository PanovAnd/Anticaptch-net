using System.Net.Http;
using System.Text;
using Anticaptcha.ApiRequests.Tasks;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    internal class CreateTaskRequest:IApiRequest{
        [JsonProperty("clientKey")]
        public readonly string ApiKey;
        
        [JsonProperty("softId")]
        public readonly int SoftId;

        [JsonProperty("task")]
        public readonly AnticaptchaTask Task;

        internal CreateTaskRequest(string apiKey, int softId, AnticaptchaTask task){
            ApiKey = apiKey;
            SoftId = softId;
            Task = task;
        }

        private const string _anticaptchaEndpoint = @"https://api.anti-captcha.com/createTask";
        public HttpRequestMessage CreateHttpRequest(){
            var request = new HttpRequestMessage(HttpMethod.Post, _anticaptchaEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return request;
        }

        
    }
    
    public class CreateTaskResponse : ErrorResponse{
        [JsonProperty("taskId")]
        public int TaskId;
    }
}