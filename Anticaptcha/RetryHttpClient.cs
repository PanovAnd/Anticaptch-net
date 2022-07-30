﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using Polly;

namespace Anticaptcha{
    
    internal class RetryHttpClient{
        
        private readonly HttpClient _httpClient;
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(200);
        
        public RetryHttpClient(){
            IAsyncPolicy<HttpResponseMessage> retryAndTimeoutPolicy = Polly.Retry.RetryPolicy<HttpResponseMessage>.Handle<Exception>()
                                                                            .OrResult(p => !p.IsSuccessStatusCode)
                                                                            .RetryAsync(3, (exception, retryNumber) => Task.Delay(Delay.Multiply(retryNumber)).Wait())
                                                                            .WrapAsync(Policy.TimeoutAsync(Timeout));

            _httpClient = new HttpClient(new PolicyHttpMessageHandler(retryAndTimeoutPolicy) {
                InnerHandler = new HttpClientHandler() {
                    AllowAutoRedirect = true,
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                }
            }) { Timeout = System.Threading.Timeout.InfiniteTimeSpan, };
        }

        private async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage requestMessage,CancellationToken cancellationToken){
            if (requestMessage is null) throw new AggregateException();

            return await _httpClient.SendAsync(requestMessage,cancellationToken);
        }

        public async Task<T> GetResponseAsJsonAsync<T>(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken){
            var responseMessage = await GetResponseAsync(requestMessage, cancellationToken);

            try{
                return JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());
            }catch (Exception e){
                throw new Exception();
            }
        }

    }
}