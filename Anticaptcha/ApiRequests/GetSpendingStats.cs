using Anticaptcha.JsonHelpers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Anticaptcha.ApiRequests {
    internal class GetSpendingStats : AuthorizedRequest, IApiRequest {
        [JsonProperty(PropertyName = "queueId", NullValueHandling = NullValueHandling.Ignore)]
        internal readonly QueueType? QueueType;

        [JsonProperty(PropertyName = "date", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UnixTimeStampToDatetimeOffsetConvertor))]
        internal readonly DateTimeOffset? TimePeriodStart;

        [JsonProperty(PropertyName = "ip", NullValueHandling = NullValueHandling.Ignore)]
        internal readonly IPAddress TargetIP;

        public GetSpendingStats(AuthorizeInfo authorizeInfo, QueueType? queueType, DateTimeOffset? timePeriodStart, IPAddress ipAddress) : base(authorizeInfo) {
            QueueType = queueType;
            TimePeriodStart = timePeriodStart;
            TargetIP = ipAddress;
        }

        private const string _anticaptchaEndpoint = "https://api.anti-captcha.com/getSpendingStats";
        public HttpRequestMessage CreateHttpRequest() {
            var request = new HttpRequestMessage(HttpMethod.Post, _anticaptchaEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return request;
        }
    }

    public class GetSpendingStatsResult : ErrorResponse {
        [JsonProperty(PropertyName = "data")]
        public SpendingStats[] Data { get; private set; }
    }

    public class SpendingStats {
        /// <summary>
        /// beginning of record period
        /// </summary>
        [JsonProperty("dateFrom")]
        [JsonConverter(typeof(UnixTimeStampToDatetimeOffsetConvertor))]
        public DateTimeOffset DateFrom { get; private set; }
        /// <summary>
        /// end of record period
        /// </summary>
        [JsonProperty("dateTill")]
        [JsonConverter(typeof(UnixTimeStampToDatetimeOffsetConvertor))]
        public DateTimeOffset DateTill { get; private set; }
        /// <summary>
        /// amount of tasks
        /// </summary>
        [JsonProperty("volume")]
        public int Volume { get; private set; }
        /// <summary>
        /// funds spent on tasks
        /// </summary>
        [JsonProperty("money")]
        public decimal Money { get; private set; }
    }

}
