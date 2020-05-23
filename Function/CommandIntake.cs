using System;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Events;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Function
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, PointsEvent> _eventsFactory;
        private readonly ConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, ConnectionMultiplexer redis, IConfiguration configuration)
        {
            _eventsFactory = eventsFactory;
            _redis = redis;
            _configuration = configuration;
        }

        [FunctionName("CommandIntake")]
        public Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload)
        {
            var dataBase = _redis.GetDatabase();
            var command = JsonDocument.Parse(commandPayload);

            var newEvent = _eventsFactory(command);
            if (newEvent == null) return Task.CompletedTask;
            if (newEvent.EventParameters.Amount <= 0 || newEvent.EventParameters.Amount > Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"])) return Task.CompletedTask;

            var rootKey = $"{newEvent.Root}";
            var playerKey = $"{newEvent.Root}_{newEvent.TargetPlayerId}";
            var timeoutKey = $"{newEvent.Root}_{newEvent.OriginPlayerId}_timeout";

            if (dataBase.StringGet(timeoutKey) != RedisValue.Null) return Task.CompletedTask;

            var newEventTasks = new Task[]
            {
                dataBase.ListRightPushAsync(rootKey, JsonSerializer.Serialize(newEvent)),
                dataBase.ListRightPushAsync(playerKey,
                    JsonSerializer.Serialize(newEvent.EventParameters)),
                dataBase.StringSetAsync(timeoutKey, new RedisValue("--expirykey--"), TimeSpan.FromSeconds(Double.Parse(_configuration["CommandTimeoutInSeconds"])))
            };

            return Task.WhenAll(newEventTasks);
        }
    }
}
