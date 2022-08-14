using System.Net.Http;
using System.Text;
using Anticaptcha.ApiRequests.Tasks;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    internal class CreateTaskRequest:AuthorizedRequest,IApiRequest{
        [JsonProperty("task")]
        public readonly AnticaptchaTask Task;

        public CreateTaskRequest(AuthorizeInfo authorizeInfo, AnticaptchaTask task):base(authorizeInfo) {
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