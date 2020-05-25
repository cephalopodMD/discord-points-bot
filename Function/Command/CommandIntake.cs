using System;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Command.Events;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Function.Command
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, GameEvent> _eventsFactory;
        private readonly ConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        public CommandIntake(Func<JsonDocument, GameEvent> eventsFactory, ConnectionMultiplexer redis, IConfiguration configuration)
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

            var gameEvent = _eventsFactory(command);
            if (gameEvent == null) return Task.CompletedTask;
            if (gameEvent.PointsEvent.Amount <= 0 || gameEvent.PointsEvent.Amount > Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"])) return Task.CompletedTask;

            var rootKey = $"{gameEvent.Root}";
            var playerKey = $"{gameEvent.Root}_{gameEvent.TargetPlayerId}";
            var timeoutKey = $"{gameEvent.Root}_{gameEvent.OriginPlayerId}_timeout";

            if (dataBase.StringGet(timeoutKey) != RedisValue.Null) return Task.CompletedTask;

            var newEventTasks = new Task[]
            {
                dataBase.ListRightPushAsync(rootKey, JsonSerializer.Serialize(gameEvent)),
                dataBase.ListRightPushAsync(playerKey,
                    JsonSerializer.Serialize(gameEvent.PointsEvent)),
                dataBase.StringSetAsync(timeoutKey, new RedisValue("--expirykey--"), TimeSpan.FromSeconds(Double.Parse(_configuration["CommandTimeoutInSeconds"])))
            };

            return Task.WhenAll(newEventTasks);
        }
    }
}
