using System.Threading.Tasks;
using Function.Command.Events;
using PointsBot.Core.Models;

namespace Function.Query
{
    public class GameState
    {
        private readonly IEventStorage<PointsEvent> _pointsEventStorage;

        public GameState(IEventStorage<PointsEvent> pointsEventStorage)
        {
            _pointsEventStorage = pointsEventStorage;
        }

        public async Task<PlayerState> RefreshPlayer(string playerId)
        {
            var events = await _pointsEventStorage.GetEvents(playerId);
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