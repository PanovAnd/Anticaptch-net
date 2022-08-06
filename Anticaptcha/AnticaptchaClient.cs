using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Anticaptcha.ApiRequests;
using Anticaptcha.ApiRequests.Tasks;

namespace Anticaptcha{
    public class AnticaptchaClient{
        private readonly string _apiKey;
        private readonly int _softId;
        private readonly RetryHttpClient _httpClient = new RetryHttpClient();

        public AnticaptchaClient(string clientKey, int? softId = null){
            _apiKey = clientKey;
            _softId = softId ?? 0;
        }

        internal async Task<CheckTaskResponse<T>> SolveCaptchaAsync<T>(AnticaptchaTask task, CancellationToken cancellationToken) where T:ITaskResult{
            var taskId = await CreateTaskAsync(task, cancellationToken);

            CheckTaskResponse<T> res;
            do{
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                res = await CheckResponseAsync<T>(taskId, cancellationToken);
            } while (res.Status != TaskResultStatus.Ready);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBase64">Captcha image is format base64</param>
        /// <param name="cancellationToken"></param>
        public async Task<CheckTaskResponse<ImageToTextResult>> SolveCaptchaAsync(string imageBase64, CancellationToken cancellationToken) =>
            await SolveCaptchaAsync<ImageToTextResult>(new ImageToTextTask(imageBase64), cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task id</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AnticaptchaApiException"></exception>
        public async Task<int> CreateTaskAsync(AnticaptchaTask task, CancellationToken cancellationToken){
            if (task is null) throw new ArgumentException();

            var request = new CreateTaskRequest(_apiKey, _softId, task);
            var apiResponse = await _httpClient.GetResponseAsJsonAsync<CreateTaskResponse>(request.CreateHttpRequest(), cancellationToken);

            if (apiResponse.Failed) throw apiResponse.ExtractException();

            return apiResponse.TaskId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<CheckTaskResponse<T>> CheckResponseAsync<T>(int taskId, CancellationToken cancellationToken) where T:ITaskResult{
            if (taskId < 1) throw new AggregateException();

            var apiRequest = new CheckTaskRequest(_apiKey, taskId).CreateHttpRequest();

            var apiResponse = await _httpClient.GetResponseAsJsonAsync<CheckTaskResponse<T>>(apiRequest, cancellationToken);
            return apiResponse;
        }

        public async Task<double> GetBalance(CancellationToken cancellationToken){
            var apiResponse = await _httpClient.GetResponseAsJsonAsync<GetBalanceResponse>(new GetBalanceRequest(_apiKey).CreateHttpRequest(), cancellationToken);

            if (apiResponse.Failed) throw apiResponse.ExtractException();

            return apiResponse.Balance;
        }

        public async Task<GetQueueStatsResponse> GetQueueStats(QueueType queueType,CancellationToken cancellationToken){
            var apiResponse = await _httpClient.GetResponseAsJsonAsync<GetQueueStatsResponse>(new GetQueueStatsRequest(queueType).CreateHttpRequest(), cancellationToken);

            if (apiResponse.Failed) throw apiResponse.ExtractException();

            return apiResponse;
        }

    }

    [Serializable]
    public class AnticaptchaApiException : Exception{
        internal AnticaptchaApiException(){ }
        internal AnticaptchaApiException(string message) : base(message){ }
        internal AnticaptchaApiException(string message, Exception inner) : base(message, inner){ }

        internal AnticaptchaApiException(ErrorResponse response) : base(
            $"Api error code:{response.ErrorCode}\r\nError message:{response.ErrorDescription}"){ }

        protected AnticaptchaApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context){ }
    }
}