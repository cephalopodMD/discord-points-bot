using System;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Command.Events;
using Function.Query;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Function.Command
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, PointsEvent> _eventsFactory;
        private readonly IConfiguration _configuration;

        private readonly IGameTimer _gameTimer;
        private readonly IEventWriter<PointsEvent> _pointsEventWriter;

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, IConfiguration configuration, IGameTimer gameTimer, IEventWriter<PointsEvent> pointsEventWriter)
        {
            _eventsFactory = eventsFactory;
            _configuration = configuration;
            _gameTimer = gameTimer;
            _pointsEventWriter = pointsEventWriter;
        }

        [FunctionName("CommandIntake")]
        public Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload)
        {
            var command = JsonDocument.Parse(commandPayload);

            var pointsEvent = _eventsFactory(command);
            if (pointsEvent == null) return Task.CompletedTask;
            if (pointsEvent.Amount <= 0 || pointsEvent.Amount > Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"])) return Task.CompletedTask;

            var updateTasks = new[]
            {
                _pointsEventWriter.PushEvents(pointsEvent),
                _gameTimer.Timeout(pointsEvent.OriginPlayerId)
            };

            return Task.WhenAll(updateTasks);
        }
    }
}
