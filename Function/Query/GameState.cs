using System.Threading.Tasks;
using Function.Command.Events;
using PointsBot.Core.Models;

namespace Function.Query
{
    public sealed class GameState
    {
        private readonly IEventFeed<PointsEvent> _pointsEventFeed;

        public GameState(IEventFeed<PointsEvent> pointsEventFeed)
        {
            _pointsEventFeed = pointsEventFeed;
        }

        public async Task<PlayerState> RefreshPlayer(string playerId)
        {
            var events = await _pointsEventFeed.GetEvents(playerId);
            var amountOfPoints = 0;
            foreach (var pointsEvent in events)
            {
                switch (pointsEvent.Action)
                {
                    case "add":
                        amountOfPoints += pointsEvent.Amount;
                        break;
                    case "remove":
                        amountOfPoints -= pointsEvent.Amount;
                        break;
                    default: break;
                }
            }

            return new PlayerState(playerId, amountOfPoints);
        }
    }
}