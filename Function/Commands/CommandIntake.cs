using System;
using System.Collections.Generic;
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

        public CommandIntake(Func<JsonDocument, PointsEvent> eventsFactory, Func<int> maxPointsPerAction, IGameTimer gameTimer, IEventWriter<PointsEvent> pointsEventWriter)
        {
            _eventsFactory = eventsFactory;
            _maxPointsPerAction = maxPointsPerAction;
            _gameTimer = gameTimer;
            _pointsEventWriter = pointsEventWriter;
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
                _gameTimer.Timeout(pointsEvent.OriginPlayerId)
            };

            return Task.WhenAll(updateTasks);
        }

        [FunctionName("ChangeFeedProcessor")]
        [StorageAccount("PointsReadModelConnectionString")]
        public async Task ProcessChangeFeed(
            [CosmosDBTrigger("points_bot", "points_events_monitored", CreateLeaseCollectionIfNotExists = true, ConnectionStringSetting = "EventFeedConnectionString")]
            IReadOnlyList<Document> changes,
            [Table("points")] CloudTable pointsTable
        )
        {
            if (changes.Count <= 0) return;

            var playerPointsByName = new Dictionary<string, PlayerPoints>();
            foreach (var change in changes)
            {
                var pointsEvent = change.GetPropertyValue<PointsEvent>("Event");
                if (!playerPointsByName.TryGetValue(pointsEvent.TargetPlayerId, out var playerPoints))
                {
                    var retrieveAction =
                        TableOperation.Retrieve<PlayerPoints>(pointsEvent.Source, pointsEvent.TargetPlayerId);
                    var playerPointsResult = await pointsTable.ExecuteAsync(retrieveAction);

                    if (playerPointsResult.HttpStatusCode >= 500)
                    {
                        throw new WebException(
                            $"Error getting player (Source: {pointsEvent.Source}, Player: {pointsEvent.TargetPlayerId}  : {playerPointsResult.Result} ",
                            WebExceptionStatus.UnknownError);
                    }

                    if (playerPointsResult.HttpStatusCode == 404)
                    {
                        playerPoints = new PlayerPoints
                            {PartitionKey = pointsEvent.Source, RowKey = pointsEvent.TargetPlayerId, TotalPoints = 0};

                        var addAction = TableOperation.Insert(playerPoints);
                        var addResult = await pointsTable.ExecuteAsync(addAction);

                        if (addResult.HttpStatusCode >= 500)
                        {
                            throw new WebException($"Error adding new player to storage account: {addResult.Result} ",
                                WebExceptionStatus.UnknownError);
                        }

                        playerPointsByName.Add(pointsEvent.TargetPlayerId, playerPoints);
                    }
                    else
                    {
                        playerPoints = (PlayerPoints)playerPointsResult.Result;
                        playerPointsByName.Add(pointsEvent.TargetPlayerId, playerPoints);
                    }
                }

                if (pointsEvent.Action == "add") playerPoints.TotalPoints += pointsEvent.Amount;
                else if (pointsEvent.Action == "remove") playerPoints.TotalPoints -= pointsEvent.Amount;

                var mergeAction = TableOperation.Merge(playerPoints);
                var mergeResult = await pointsTable.ExecuteAsync(mergeAction);

                if (mergeResult.HttpStatusCode >= 500)
                {
                    throw new WebException($"Error updating player (Source: {pointsEvent.Source}, Player: {pointsEvent.TargetPlayerId}  : {mergeResult.Result} ",
                        WebExceptionStatus.UnknownError);
                }
            }
        }
    }

    public class PlayerPoints : TableEntity
    {
        public int TotalPoints { get; set; }
    }
}
