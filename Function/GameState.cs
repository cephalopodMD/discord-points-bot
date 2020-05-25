using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Events;
using PointsBot.Core.Models;
using StackExchange.Redis;

namespace Function
{
    public interface IEventStorage<TEvent>
    {
        Task<IEnumerable<TEvent>> GetEvents(string playerId);
    }

    public class RedisPointsEventStorage : IEventStorage<PointsEvent>
    {
        private readonly IConnectionMultiplexer _redis;

        private static string PlayerKey(string playerId) => $"points_{playerId}";

        public RedisPointsEventStorage(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<IEnumerable<PointsEvent>> GetEvents(string playerId)
        {
            var database = _redis.GetDatabase();

            var eventListLength = await database.ListLengthAsync(PlayerKey(playerId));
            var redisValueTasks = new List<Task<RedisValue>>();

            for (long ii = 0; ii <= eventListLength; ii++)
            {
                redisValueTasks.Add(database.ListGetByIndexAsync(PlayerKey(playerId), ii));
            }

            var redisValues = (await Task.WhenAll(redisValueTasks)).ToList();
            return redisValues.Select(value => JsonSerializer.Deserialize<PointsEvent>(Encoding.UTF8.GetBytes(value)));
        }
    }

    public class GameState
    {
        private readonly IEventStorage<PointsEvent> _pointsEventStorage;

        public GameState(IEventStorage<PointsEvent> pointsEventStorage)
        {
            _pointsEventStorage = pointsEventStorage;
        }

        private static string PlayerTimeoutKey(string playerId) => $"points_{playerId}_timeout";

        public async Task<PlayerState> RefreshPlayer(string playerId)
        {
            var events = await _pointsEventStorage.GetEvents(playerId);

            int amountOfPoints = 0;
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

        public bool IsPlayerTimedOut(string playerId)
        {
            return true;
            //var database = _pointsEventStorage.GetDatabase();
            //var timeoutKey = PlayerTimeoutKey(playerId);

            //return database.StringGet(timeoutKey) != RedisValue.Null;
        }
    }
}