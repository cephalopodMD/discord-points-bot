using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PointsBot.Core;
using PointsBot.Infrastructure;

namespace Infrastructure.Test.Unit
{
    [TestClass]
    public class CosmosTimerTests
    {
        [TestMethod]
        public async Task Timeout_CallsCreateItemAsync_OnCosmosContainerWhenTimeoutRecordDoesNotExist()
        {
            const string PlayerId = "TestPlayer";
            const string Source = "CosmosEventWriterUnitTests";

            var mockTimerClient = new Mock<ICosmosTimerClient>();
            mockTimerClient.Setup(timerClient =>
                timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source), It.Is<string>(s => s == PlayerId)))
                .Returns(Task.FromResult(new ResponseMessage(HttpStatusCode.NotFound)))
                .Verifiable();

            mockTimerClient.Setup(timerClient =>
                    timerClient.CreatePlayerTimeoutRecord(It.Is<string>(s => s == Source),
                        It.Is<string>(s => s == PlayerId)))
                .Returns(Task.FromResult(new ResponseMessage(HttpStatusCode.OK)))
                .Verifiable();

            var structureUnderTest = new CosmosTimer(mockTimerClient.Object);
            await structureUnderTest.Timeout(PlayerId, Source);

            mockTimerClient.Verify(
                timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source),
                        It.Is<string>(s => s == PlayerId)), Times.Once);

            mockTimerClient.Verify(timerClient => timerClient.CreatePlayerTimeoutRecord(It.Is<string>(s => s == Source),
                It.Is<string>(s => s == PlayerId)), Times.Once);
        }

        [TestMethod]
        public async Task Timeout_DoesNotCallCreateItemAsync_OnCosmosContainerWhenTimeoutRecordExists()
        {
            const string PlayerId = "TestPlayer";
            const string Source = "CosmosEventWriterUnitTests";

            var mockTimerClient = new Mock<ICosmosTimerClient>();
            mockTimerClient.Setup(timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source), It.Is<string>(s => s == PlayerId)))
                .Returns(Task.FromResult(new ResponseMessage(HttpStatusCode.OK)))
                .Verifiable();

            var structureUnderTest = new CosmosTimer(mockTimerClient.Object);
            await structureUnderTest.Timeout(PlayerId, Source);

            mockTimerClient.Verify(
                timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source),
                        It.Is<string>(s => s == PlayerId)), Times.Once);

            mockTimerClient.Verify(timerClient => timerClient.CreatePlayerTimeoutRecord(It.Is<string>(s => s == Source),
                It.Is<string>(s => s == PlayerId)), Times.Never);
        }

        [TestMethod]
        public async Task HasTimeout_ReturnsTrue_WhenTimeoutRecordExists()
        {
            const string PlayerId = "TestPlayer";
            const string Source = "CosmosEventWriterUnitTests";

            var mockTimerClient = new Mock<ICosmosTimerClient>();
            mockTimerClient.Setup(timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source), It.Is<string>(s => s == PlayerId)))
                .Returns(Task.FromResult(new ResponseMessage(HttpStatusCode.OK)))
                .Verifiable();

            var structureUnderTest = new CosmosTimer(mockTimerClient.Object);
            var result = await structureUnderTest.HasTimeout(PlayerId, Source);

            mockTimerClient.Verify(
                timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source),
                        It.Is<string>(s => s == PlayerId)), Times.Once);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task HasTimeout_ReturnsFalse_WhenTimeoutRecordDoesNotExist()
        {
            const string PlayerId = "TestPlayer";
            const string Source = "CosmosEventWriterUnitTests";

            var mockTimerClient = new Mock<ICosmosTimerClient>();
            mockTimerClient.Setup(timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source), It.Is<string>(s => s == PlayerId)))
                .Returns(Task.FromResult(new ResponseMessage(HttpStatusCode.NotFound)))
                .Verifiable();

            var structureUnderTest = new CosmosTimer(mockTimerClient.Object);
            var result = await structureUnderTest.HasTimeout(PlayerId, Source);

            mockTimerClient.Verify(
                timerClient =>
                    timerClient.GetPlayerTimeoutRecord(It.Is<string>(s => s == Source),
                        It.Is<string>(s => s == PlayerId)), Times.Once);

            Assert.IsFalse(result);
        }
    }
}
