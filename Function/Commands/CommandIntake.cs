using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Events;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PointsBot.Core;

namespace Function.Commands
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, PointsEvent> _eventsFactory;
        private readonly IGameTimer _gameTimer;
        private readonly IEventWriter<PointsEvent> _pointsEventWriter;
        private readonly Func<int> _maxPointsPerAction;
        private readonly PlayerCache _playerCache;

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, Func<int> maxPointsPerAction, IGameTimer gameTimer, IEventWriter<PointsEvent> pointsEventWriter, PlayerCache playerCache)
        {
            _eventsFactory = eventsFactory;
            _maxPointsPerAction = maxPointsPerAction;
            _gameTimer = gameTimer;
            _pointsEventWriter = pointsEventWriter;
            _playerCache = playerCache;
        }
        
        [FunctionName("CommandIntake")]
        public Task InterpretCommand([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload)
        {
            var command = JsonDocument.Parse(commandPayload);

            var pointsEvent = _eventsFactory(command);
            if (pointsEvent == null) return Task.CompletedTask;
            if (pointsEvent.Amount <= 0 || pointsEvent.Amount > _maxPointsPerAction()) return Task.CompletedTask;

            var updateTasks = new[]
            {
                _pointsEventWriter.PushEvents(pointsEvent),
                _gameTimer.Timeout(pointsEvent.OriginPlayerId, pointsEvent.Source)
            };

            return Task.WhenAll(updateTasks);
        }

        [FunctionName("ChangeFeedProcessor")]
        [StorageAccount("PointsReadModelConnectionString")]
        public async Task ProcessChangeFeed(
            [CosmosDBTrigger("points_bot", "points_events_monitored", CreateLeaseCollectionIfNotExists = true, ConnectionStringSetting = "CosmosConnectionString")]
            IReadOnlyList<Document> changes,
            [Table("points")] CloudTable pointsTable
        )
        {
            if (changes.Count <= 0) return;

            var updatedPlayers = new List<PlayerPoints>();
            foreach (var change in changes)
            {
                var playerId = ParseId(change.Id);
                var playerPoints = await _playerCache.GetPlayer(pointsTable, playerId);

                var pointsEvents = change.GetPropertyValue<IEnumerable<PointsEvent>>("Events").ToList();
                if (pointsEvents.Count == playerPoints.LastEventIndex) continue;

                var eventIndex = playerPoints.LastEventIndex + 1;
                do
                {
                    var pointsEvent = pointsEvents[eventIndex];

                    if (pointsEvent.Action == "add") playerPoints.TotalPoints += pointsEvent.Amount;
                    else if (pointsEvent.Action == "remove") playerPoints.TotalPoints -= pointsEvent.Amount;

                    eventIndex++;

                } while (eventIndex < pointsEvents.Count);
                playerPoints.LastEventIndex = eventIndex - 1;

                var mergeAction = TableOperation.Merge(playerPoints);
                var mergeResult = await pointsTable.ExecuteAsync(mergeAction);

                if (mergeResult.HttpStatusCode >= 500)
                {
                    throw new WebException($"Error updating player (Player: {playerId}  : {mergeResult.Result} ",
                        WebExceptionStatus.UnknownError);
                }

                updatedPlayers.Add(playerPoints);
            }

            _playerCache.UpdatePlayer(updatedPlayers);
        }

        private static string ParseId(string documentId)
        {
            var splitId = documentId.Split('_');

            if (splitId.Length != 3) throw new Exception("Document Id is malformed.");
            var source = $"{splitId[0]}_{splitId[1]}";
            var targetPlayerId = splitId[2];

            return $"{source}_{targetPlayerId}";
        }
    }
}
