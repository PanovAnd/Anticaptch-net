using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anticaptcha.Tests {
    public class HttpClientMock : IInterceptor {

        public void Intercept(IInvocation invocation) {
            if (invocation.Arguments.Length != 2 || !(invocation.Arguments[0] is HttpRequestMessage requestMessage))
                throw new Exception();

            Task<HttpResponseMessage> response;
            switch (requestMessage.RequestUri.AbsolutePath.ToLower()) {
                case "/getbalance":
                    response = GetBalance();
                    break;
                case "/getqueuestats":
                    response = GetQueueStat();
                    break;
                case "/getspendingstats":
                    response = GetSpendingStats();
                    break;
                case "/createtask":
                    response = CreateTask();
                    break;
                case "/gettaskresult":
                    response = GetTaskResult();
                    break;
                default: throw new Exception();
            }
            invocation.ReturnValue = response;
            return;
        }

        private async Task<HttpResponseMessage> GetTaskResult() => new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"errorId\":0,\"status\":\"ready\",\"solution\":{\"text\":\"test\",\"url\":\"http://test.test/test.jpg\"},\"cost\":\"0.000700\",\"ip\":\"46.98.54.221\",\"createTime\":1472205564,\"endTime\":1472205570,\"solveCount\":\"1\"}") };
        

        private async Task<HttpResponseMessage> CreateTask() => new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"errorId\": 0,\"taskId\": 1}") };
        

        private async Task<HttpResponseMessage> GetSpendingStats() => new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"errorId\":0,\"data\":[{\"dateFrom\":1550533500,\"dateTill\":1550537099,\"volume\":1899,\"money\":1.234},{\"dateFrom\":1550533500,\"dateTill\":1550537099,\"volume\":1899,\"money\":1.234}]}") };
        

        private async Task<HttpResponseMessage> GetQueueStat() => new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"waiting\":242,\"load\":60.33,\"bid\":\"0.0008600982\",\"speed\":10.77,\"total\": 610}") };
        

        private async Task<HttpResponseMessage> GetBalance() => new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"errorId\": 0,\"balance\": 12.3456}") };
        
    }
}
