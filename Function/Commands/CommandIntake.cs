using System;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Events;
using Microsoft.Azure.WebJobs;
using PointsBot.Core;

namespace Function.Commands
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, PointsEvent> _eventsFactory;
        private readonly IGameTimer _gameTimer;
        private readonly IEventWriter<PointsEvent> _pointsEventWriter;
        private readonly Func<int> _maxPointsPerAction;

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, Func<int> maxPointsPerAction, IGameTimer gameTimer, IEventWriter<PointsEvent> pointsEventWriter)
        {
            _eventsFactory = eventsFactory;
            _maxPointsPerAction = maxPointsPerAction;
            _gameTimer = gameTimer;
            _pointsEventWriter = pointsEventWriter;
        }
        
        [FunctionName("CommandIntake")]
        public Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload)
        {
            var command = JsonDocument.Parse(commandPayload);

            var pointsEvent = _eventsFactory(command);
            if (pointsEvent == null) return Task.CompletedTask;
            if (pointsEvent.Amount <= 0 || pointsEvent.Amount > _maxPointsPerAction()) return Task.CompletedTask;

            var updateTasks = new[]
            {
                _pointsEventWriter.PushEvents(pointsEvent),
                _gameTimer.Timeout(pointsEvent.OriginPlayerId)
            };

            return Task.WhenAll(updateTasks);
        }
    }
}
