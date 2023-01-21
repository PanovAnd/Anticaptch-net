using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Anticaptcha.Tests {
    class HttpClientMockGetLastRequest : IInterceptor {

        public string LastRequestPath;
        public string LastRequestJson;

        public void Intercept(IInvocation invocation) {
            if (invocation.Arguments.Length != 2 || !(invocation.Arguments[0] is HttpRequestMessage requestMessage))
                throw new Exception();

            switch (requestMessage.RequestUri.AbsolutePath.ToLower()) {
                case "/getbalance":
                case "/getqueuestats":
                    break;
                //case "/getspendingstats":
                //    response = GetSpendingStats();
                //    break;
                //case "/createtask":
                //    response = CreateTask();
                //    break;
                //case "/gettaskresult":
                //    response = GetTaskResult();
                //    break;
                default: throw new Exception();
            }
            LastRequestPath = requestMessage.RequestUri.AbsolutePath;
            LastRequestJson = ExtractJsonRequest(requestMessage);
            invocation.ReturnValue = Task.Run(()=> new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
            return;
        }

        private string ExtractJsonRequest(HttpRequestMessage requestMessage) {
            const string jsonMediaType = "application/json";
            if (requestMessage.Content.Headers.ContentType.MediaType != jsonMediaType) throw new Exception();

            return requestMessage.Content.ReadAsStringAsync().Result;
        }


    }
}
