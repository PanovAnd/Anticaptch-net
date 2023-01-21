using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Anticaptcha.Tests {
    public class RequestsTests {
        [Fact(DisplayName = "GetBalance request message test")]
        public async Task GetBalanceCheckRequestMessage() {
            var interceptor = new HttpClientMockGetLastRequest();
            string clientKeyValue = "test";

            var mockHttpClient = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy<HttpClient>(interceptor);
            var anticaptchaClient = new AnticaptchaClient(clientKeyValue, mockHttpClient);

            await Assert.ThrowsAsync<NullReferenceException>(async ()=> await anticaptchaClient.GetBalance(CancellationToken.None));
            Assert.True(interceptor.LastRequestPath.ToLower() == "/getbalance", $"{nameof(interceptor.LastRequestPath)} is wrong");
            Assert.True(!string.IsNullOrEmpty(interceptor.LastRequestJson));

            var lastRequestJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(interceptor.LastRequestJson);
            CheckJsonKeyExistAndValue(lastRequestJson, "clientKey", clientKeyValue);
        }

        [Fact(DisplayName = "GetQueueStat request message test")]
        public async Task GetQueueStatRequestMessage() {
            var interceptor = new HttpClientMockGetLastRequest();
            var clientKeyValue = "test";
            var queueType= ApiRequests.QueueType.HCaptcha;

            var mockHttpClient = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy<HttpClient>(interceptor);
            var anticaptchaClient = new AnticaptchaClient(clientKeyValue, mockHttpClient);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await anticaptchaClient.GetQueueStats(queueType, CancellationToken.None));
            Assert.True(interceptor.LastRequestPath.ToLower() == "/getqueuestats", $"{nameof(interceptor.LastRequestPath)} is wrong");
            Assert.True(!string.IsNullOrEmpty(interceptor.LastRequestJson));

            var lastRequestJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(interceptor.LastRequestJson);
            
            CheckJsonKeyExistAndValue(lastRequestJson, "queueId", ((int)queueType).ToString());
        }


        private void CheckJsonKeyExistAndValue(Dictionary<string,string> json,string key,string value) {
            Assert.True(json.ContainsKey(key));
            Assert.True(json[key] == value);
        }


    }
}
