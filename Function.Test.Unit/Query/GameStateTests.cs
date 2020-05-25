using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Function.Events;
using Function.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Function.Test.Unit.Query
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public async Task RefreshPlayer_ReturnsAPlayerState_WithMatchingPlayerId()
        {
            const string PlayerId = "PlayerId";

            var mockEventFeed = new Mock<IEventFeed<PointsEvent>>();
            mockEventFeed.Setup(eventFeed => eventFeed.GetEvents(PlayerId))
                .Returns(Task.FromResult(Enumerable.Empty<PointsEvent>()));

            var structureUnderTest = new GameState(mockEventFeed.Object);

            var result = await structureUnderTest.RefreshPlayer(PlayerId);

            Assert.AreEqual(result.PlayerId, PlayerId);
        }

        [TestMethod]
        public async Task RefreshPlayer_ReturnsAPlayerState_WithACorrectTotalNumberOfPoints()
        {
            const string PlayerId = "PlayerId";
            var pointsEvents = new[]
            {
                new PointsEvent
                {
                    Action = "add",
                    Amount = 20
                },
                new PointsEvent
                {
                    Action = "remove",
                    Amount = 10
                },
                new PointsEvent
                {
                    Action = "add",
                    Amount = 50
                },
                new PointsEvent
                {
                    Action = "remove",
                    Amount = 30
                }
            }.ToList();

            var mockEventFeed = new Mock<IEventFeed<PointsEvent>>();
            mockEventFeed.Setup(eventFeed => eventFeed.GetEvents(PlayerId))
                .Returns(Task.FromResult<IEnumerable<PointsEvent>>(pointsEvents));

            var structureUnderTest = new GameState(mockEventFeed.Object);

            var result = await structureUnderTest.RefreshPlayer(PlayerId);

            Assert.AreEqual(result.TotalPoints, 30);
        }

    }
}