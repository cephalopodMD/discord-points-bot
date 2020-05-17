using System;
using System.Text.Json;
using System.Threading.Tasks;
using CommandsFunction.Events;
using Microsoft.Azure.WebJobs;
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
        public Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload, ILogger log)
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
}
