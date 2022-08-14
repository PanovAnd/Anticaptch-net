using Anticaptcha.ApiRequests.Tasks;
using Anticaptcha.JsonHelpers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace Anticaptcha.ApiRequests {
    internal class CheckTaskRequest : AuthorizedRequest, IApiRequest {
        [JsonProperty("taskId")]
        public readonly int TaskId;

        public CheckTaskRequest(string apiKey, int taskId) : base(new AuthorizeInfo(apiKey, null)) {
            if (taskId < 1) throw new ArgumentException();

            TaskId = taskId;
        }

        private const string _anticaptchaEndpoint = @"https://api.anti-captcha.com/getTaskResult";
        public HttpRequestMessage CreateHttpRequest() {
            var a = new HttpRequestMessage(HttpMethod.Post, _anticaptchaEndpoint);
            a.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return a;
        }
    }

    public enum TaskResultStatus {
        Unknown = 0,
        [JsonProperty("processing")]
        Processing,
        [JsonProperty("ready")]
        Ready,
    }

    public class CheckTaskResponse<T> : ErrorResponse where T : ITaskResult {
        [JsonProperty("status")]
        public TaskResultStatus Status { get; private set; }

        [JsonProperty("cost")]
        public decimal Cost { get; private set; }

        [JsonProperty("ip")]
        public string SenderIp { get; private set; }

        [JsonProperty("createTime")]
        [JsonConverter(typeof(UnixTimeStampToDatetimeOffsetConvertor))]
        public DateTimeOffset CreateTime { get; private set; }

        [JsonProperty("endTime")]
        [JsonConverter(typeof(UnixTimeStampToDatetimeOffsetConvertor))]
        public DateTimeOffset EndTime { get; private set; }

        [JsonProperty("solveCount")]
        public int SolveCount { get; private set; }

        [JsonProperty("solution")]
        public T Solution { get; private set; }
    }
}