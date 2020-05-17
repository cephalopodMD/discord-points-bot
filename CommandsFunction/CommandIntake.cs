using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommandsFunction.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CommandsFunction
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, PointsEvent> _eventsFactory;
        private readonly ConnectionMultiplexer _redis;

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, ConnectionMultiplexer redis)
        {
            _eventsFactory = eventsFactory;
            _redis = redis;
        }

        [FunctionName("CommandIntake")]
        public Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload)
        {
            var dataBase = _redis.GetDatabase();
            var command = JsonDocument.Parse(commandPayload);

            var newEvent = _eventsFactory(command);
            if (newEvent == null) return Task.CompletedTask;

            var newEventTasks = new[]
            {
                dataBase.ListRightPushAsync($"{newEvent.Root}", JsonSerializer.Serialize(newEvent)),
                dataBase.ListRightPushAsync($"{newEvent.Root}_{newEvent.PlayerId}",
                    JsonSerializer.Serialize(newEvent.EventParameters))
            };

            return Task.WhenAll(newEventTasks);
        }

        
    }

    public class QueryIntake
    {
        private readonly ConnectionMultiplexer _redis;

        public QueryIntake(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private static readonly Dictionary<string, PlayerState> PlayerStateByRedisKey = new Dictionary<string, PlayerState>();

        [FunctionName("QueryIntake")]
        public async Task<PlayerState> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "points/{playerId}")]HttpRequest request, string playerId)
        {
            var database = _redis.GetDatabase();

            var redisKey = $"points_{playerId}";
            PlayerState playerstate;
            if (PlayerStateByRedisKey.ContainsKey(redisKey)) playerstate = PlayerStateByRedisKey[redisKey];
            else
            {
                PlayerStateByRedisKey.Add(redisKey, new PlayerState{NumberOfEvents = 0, PlayerId = playerId, TotalPoints = 0});
                playerstate = PlayerStateByRedisKey[redisKey];
            }

            
            var eventListLength = database.ListLength(redisKey);
            if (playerstate.NumberOfEvents == eventListLength) return playerstate;


            var startingIndex = playerstate.NumberOfEvents;
            var redisValueTasks = new List<Task<RedisValue>>();
            while (startingIndex < eventListLength)
            {
                redisValueTasks.Add(database.ListGetByIndexAsync(redisKey, startingIndex));
                startingIndex++;
            }

            var redisValues = await Task.WhenAll(redisValueTasks);
            playerstate.NumberOfEvents = eventListLength;
            foreach (var value in redisValues)
            {
                var parameters = JsonSerializer.Deserialize<PointsEventParameters>(Encoding.UTF8.GetBytes(value));
                switch (parameters.Action)
                {
                    case "add":
                        playerstate.TotalPoints += parameters.Amount;
                        break;
                    case "remove":
                        playerstate.TotalPoints -= parameters.Amount;
                        break;
                    default: break;
                }
            }

            return playerstate;
        }

        public class PlayerState
        {
            public long NumberOfEvents { get; set; }

            public string PlayerId { get; set; }

            public int TotalPoints { get; set; }
        }
    }
}
