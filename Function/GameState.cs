using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Events;
using PointsBot.Core.Models;
using StackExchange.Redis;

namespace Function
{
    public class GameState
    {
        private readonly IConnectionMultiplexer _redis;

        public GameState(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private readonly Dictionary<string, PlayerState> _playerStateByRedisKey =
            new Dictionary<string, PlayerState>();

        private static string PlayerKey(string playerId) => $"points_{playerId}";
        private static string PlayerTimeoutKey(string playerId) => $"points_{playerId}_timeout";

        public async Task<PlayerState> RefreshPlayer(string playerId)
        {
            var database = _redis.GetDatabase();
            var playerState = GetPlayer(playerId);
            var playerKey = PlayerKey(playerId);

            var eventListLength = await database.ListLengthAsync(playerId);
            if (playerState.NumberOfEvents == eventListLength) return playerState;

            var startingIndex = playerState.NumberOfEvents;
            var redisValueTasks = new List<Task<RedisValue>>();
            while (startingIndex < eventListLength)
            {
                redisValueTasks.Add(database.ListGetByIndexAsync(playerKey, startingIndex));
                startingIndex++;
            }

            var redisValues = await Task.WhenAll(redisValueTasks);

            playerState.NumberOfEvents = eventListLength;
            foreach (var value in redisValues)
            {
                var parameters = JsonSerializer.Deserialize<PointsEventParameters>(Encoding.UTF8.GetBytes(value));
                switch (parameters.Action)
                {
                    case "add":
                        playerState.TotalPoints += parameters.Amount;
                        break;
                    case "remove":
                        playerState.TotalPoints -= parameters.Amount;
                        break;
                    default: break;
                }
            }

            return playerState;
        }

        private PlayerState GetPlayer(string playerId)
        {
            var playerKey = PlayerKey(playerId);
            if (_playerStateByRedisKey.ContainsKey(playerKey)) return _playerStateByRedisKey[playerKey];

            AddPlayer(playerKey);
            return _playerStateByRedisKey[playerKey];
        }

        private void AddPlayer(string playerKey)
        {
            _playerStateByRedisKey.Add(PlayerKey(playerKey), new PlayerState { NumberOfEvents = 0, PlayerId = playerKey, TotalPoints = 0 });
        }

        public bool IsPlayerTimedOut(string playerId)
        {
            var database = _redis.GetDatabase();
            var timeoutKey = PlayerTimeoutKey(playerId);

            return database.StringGet(timeoutKey) != RedisValue.Null;
        }
    }
}