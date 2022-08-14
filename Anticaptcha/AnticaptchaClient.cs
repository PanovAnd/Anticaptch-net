using Anticaptcha.ApiRequests;
using Anticaptcha.ApiRequests.Tasks;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Anticaptcha {

    internal struct AuthorizeInfo {
        public readonly string ApiKey;
        public readonly int? SoftId;

        public AuthorizeInfo(string apiKey, int? softId) {
            ApiKey = apiKey;
            SoftId = softId;
        }
    }


    public class AnticaptchaClient {
        private readonly AuthorizeInfo authorizeInfo;
        private readonly RetryHttpClient _httpClient = new RetryHttpClient();

        public AnticaptchaClient(string clientKey, int? softId = null) {
            authorizeInfo = new AuthorizeInfo(clientKey, softId);
        }

        internal async Task<CheckTaskResponse<T>> SolveCaptchaAsync<T>(AnticaptchaTask task, CancellationToken cancellationToken) where T : ITaskResult {
            ArgumentChecker.ThrowIfNull(task, nameof(task));
            var taskId = await CreateTaskAsync(task, cancellationToken);

            CheckTaskResponse<T> res;
            do {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                res = await CheckResponseAsync<T>(taskId, cancellationToken);
            } while (res.Status != TaskResultStatus.Ready);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBase64">Captcha image as string formatted base64</param>
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
        public async Task<int> CreateTaskAsync(AnticaptchaTask task, CancellationToken cancellationToken) {
            ArgumentChecker.ThrowIfNull(task, nameof(task));

            CreateTaskResponse apiResponse = await GetResponse<CreateTaskResponse>(new CreateTaskRequest(authorizeInfo, task), cancellationToken);

            return apiResponse.TaskId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<CheckTaskResponse<T>> CheckResponseAsync<T>(int taskId, CancellationToken cancellationToken) where T : ITaskResult {
            if (taskId < 1) throw new AggregateException();

            return await GetResponse<CheckTaskResponse<T>>(new CheckTaskRequest(authorizeInfo.ApiKey, taskId), cancellationToken);
        }

        public async Task<double> GetBalance(CancellationToken cancellationToken) {
            var apiResponse = await _httpClient.GetResponseAsJsonAsync<GetBalanceResponse>(new GetBalanceRequest(authorizeInfo.ApiKey).CreateHttpRequest(), cancellationToken);

            if (apiResponse.Failed) throw apiResponse.ExtractException();

            return apiResponse.Balance;
        }

        public async Task<GetQueueStatsResponse> GetQueueStats(QueueType queueType, CancellationToken cancellationToken)
            => await GetResponse<GetQueueStatsResponse>(new GetQueueStatsRequest(queueType), cancellationToken);

        public async Task<GetSpendingStatsResult> GetSpendingStats(CancellationToken cancellationToken) => await GetSpendingStats(null,null,null,cancellationToken);
        public async Task<GetSpendingStatsResult> GetSpendingStats(QueueType? queueType, CancellationToken cancellationToken) => await GetSpendingStats(queueType, null, null, cancellationToken);
        public async Task<GetSpendingStatsResult> GetSpendingStats(QueueType? queueType, DateTimeOffset? periodStart, IPAddress targetIP, CancellationToken cancellationToken) 
            => await GetResponse<GetSpendingStatsResult>(new GetSpendingStats(authorizeInfo, queueType, periodStart, targetIP), cancellationToken);

        private async Task<T> GetResponse<T>(IApiRequest request,CancellationToken cancellationToken) where T : ErrorResponse {
            ArgumentChecker.ThrowIfNull(request, nameof(request));

            var apiResponse = await _httpClient.GetResponseAsJsonAsync<T>(request.CreateHttpRequest(), cancellationToken);

            if (apiResponse.Failed) throw apiResponse.ExtractException();

            return apiResponse;
        }
    }

    [Serializable]
    public class AnticaptchaApiException : Exception {
        internal AnticaptchaApiException() { }
        internal AnticaptchaApiException(string message) : base(message) { }
        internal AnticaptchaApiException(string message, Exception inner) : base(message, inner) { }

        internal AnticaptchaApiException(ErrorResponse response) : base(
            $"Api error code:{response.ErrorCode}\r\nError message:{response.ErrorDescription}") { }

        protected AnticaptchaApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    internal class ArgumentChecker {

        public static void ThrowIfNull(object parameter, string parameterName = default) {
            if (parameter is null) throw new ArgumentNullException(parameterName);
        }

    }

}