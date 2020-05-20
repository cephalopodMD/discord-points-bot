using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommandsFunction.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PointsBot.Core;
using StackExchange.Redis;

namespace CommandsFunction
{
    public class QueryIntake
    {
        private readonly ConnectionMultiplexer _redis;

        public QueryIntake(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private static readonly Dictionary<string, PlayerState> PlayerStateByRedisKey =
            new Dictionary<string, PlayerState>();

        [FunctionName("GetPlayerPoints")]
        public async Task<PlayerState> GetPlayerPoints(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "points/{playerId}")]
            HttpRequest request, string playerId)
        {
            var database = _redis.GetDatabase();
            var redisKey = $"points_{playerId}";

            PlayerState playerState;
            if (PlayerStateByRedisKey.ContainsKey(redisKey)) playerState = PlayerStateByRedisKey[redisKey];
            else
            {
                PlayerStateByRedisKey.Add(redisKey,
                    new PlayerState {NumberOfEvents = 0, PlayerId = playerId, TotalPoints = 0});
                playerState = PlayerStateByRedisKey[redisKey];
            }


            var eventListLength = database.ListLength(redisKey);
            if (playerState.NumberOfEvents == eventListLength) return playerState;


            var startingIndex = playerState.NumberOfEvents;
            var redisValueTasks = new List<Task<RedisValue>>();
            while (startingIndex < eventListLength)
            {
                redisValueTasks.Add(database.ListGetByIndexAsync(redisKey, startingIndex));
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

        [FunctionName("GetTimeout")]
        public async Task<object> GetPlayerTimeout(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "timeout/{playerId}")]
            HttpRequest request, string playerId)
        {
            var database = _redis.GetDatabase();
            var redisKey = $"points_{playerId}_timeout";

            return database.StringGet(redisKey) == RedisValue.Null ? null : "timedout";
        }
    }
}