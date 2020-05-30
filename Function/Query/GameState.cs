using System.Linq;
using System.Threading.Tasks;
using Function.Events;
using PointsBot.Core;
using PointsBot.Infrastructure.Models;

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
            return events.Aggregate(new PlayerState(playerId, 0), (state, pointsEvent) =>
            {
                switch (pointsEvent.Action)
                {
                    case "add":
                        return new PlayerState(playerId, state.TotalPoints + pointsEvent.Amount);
                    case "remove":
                        return new PlayerState(playerId, state.TotalPoints - pointsEvent.Amount);
                    default: return state;
                }
            });
        }
    }
}