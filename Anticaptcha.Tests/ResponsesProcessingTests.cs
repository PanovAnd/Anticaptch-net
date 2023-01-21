using Anticaptcha.ApiRequests.Tasks;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Anticaptcha.Tests {
    public class ResponsesProcessingTests {
        private readonly AnticaptchaClient anticaptcha;
        public ResponsesProcessingTests() {
            HttpClient mockClient = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy<HttpClient>(new HttpClientMock());

            anticaptcha = new AnticaptchaClient("testkey", mockClient);
        }

        [Fact(DisplayName = @"get balace")]
        public async Task GetBalance() {
            var balance = await anticaptcha.GetBalance(CancellationToken.None);

            Assert.True(balance == 12.3456, "Answer is wrong");
        }

        [Fact(DisplayName = "get queue stat")]
        public async Task GetQueueStat() {
            var queueState = await anticaptcha.GetQueueStats(ApiRequests.QueueType.ImageToTextRussian, CancellationToken.None);

            Assert.True(queueState != null, "Answer is null");
            Assert.True(queueState.Waiting == 242, $"{nameof(queueState.Waiting)} is wrong");
            Assert.True(queueState.Load == 60.33, $"{nameof(queueState.Load)} is wrong");
            Assert.True(queueState.Bid == 0.0008600982, $"{nameof(queueState.Bid)} is wrong");
            Assert.True(queueState.Speed == 10.77, $"{nameof(queueState.Speed)} is wrong");
            Assert.True(queueState.Total == 610, $"{nameof(queueState.Total)} is wrong");
        }

        [Fact(DisplayName = "get spending stat")]
        public async Task GetSpendingStat() {
            var queueState = await anticaptcha.GetSpendingStats(ApiRequests.QueueType.ImageToTextRussian, CancellationToken.None);

            Assert.True(queueState != null, "Answer is null");
            Assert.True(queueState.Data.Length == 2, $"{nameof(queueState.Data.Length)} is wrong");

            foreach (ApiRequests.SpendingStats spendingStats in queueState.Data) {
                Assert.True(spendingStats.DateFrom == new DateTimeOffset(2019, 02, 18, 23, 45, 0, TimeSpan.Zero), $"{nameof(spendingStats.DateFrom)} is wrong");
                Assert.True(spendingStats.DateTill == new DateTimeOffset(2019, 02, 19, 0, 44, 59, TimeSpan.Zero), $"{nameof(spendingStats.DateTill)} is wrong");
                Assert.True(spendingStats.Money == 1.234m, $"{nameof(spendingStats.Money)} is wrong");
                Assert.True(spendingStats.Volume == 1899, $"{nameof(spendingStats.Volume)} is wrong");
            }
        }

        [Fact(DisplayName = "get task result")]
        public async Task CheckTaskResult() {
            var taskResult = await anticaptcha.CheckResponseAsync<ImageToTextResult>(5, CancellationToken.None);

            Assert.False(taskResult.Solution is null, $"{nameof(taskResult.Solution)} is null");
            Assert.True(taskResult.Solution.Text == "test", $"{nameof(taskResult.Solution.Text)} is wrong");
            Assert.True(taskResult.Solution.Url == "http://test.test/test.jpg", $"{nameof(taskResult.Solution.Url)} is wrong");
            Assert.True(taskResult.Cost == 0.0007m, $"{nameof(taskResult.Cost)} is wrong");
            Assert.True(taskResult.SenderIp == "46.98.54.221", $"{nameof(taskResult.SenderIp)} is wrong");
            Assert.True(taskResult.CreateTime == new DateTimeOffset(2016, 08, 26, 9, 59, 24, TimeSpan.Zero), $"{nameof(taskResult.CreateTime)} is wrong");
            Assert.True(taskResult.EndTime == new DateTimeOffset(2016, 08, 26, 9, 59, 30, TimeSpan.Zero), $"{nameof(taskResult.EndTime)} is wrong");
            Assert.True(taskResult.SolveCount == 1, $"{nameof(taskResult.SolveCount)} is wrong");
        }

        [Fact(DisplayName = "get cretate task result")]
        public async Task CreateTask() {
            var createTaskResult = await anticaptcha.CreateTaskAsync(new ImageToTextTask(""), CancellationToken.None);

            Assert.True(createTaskResult == 1, "Answer is wrong");
        }

    }
}
