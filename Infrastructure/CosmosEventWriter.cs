using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PointsBot.Core;

namespace PointsBot.Infrastructure
{
    public class CosmosEventWriter : IEventWriter<PointsEvent>
    {
        private readonly ICosmosPointsClient _pointsClient;
        public CosmosEventWriter(ICosmosPointsClient pointsClient) { _pointsClient = pointsClient; }

        public async Task PushEvents(PointsEvent pointsEvent)
        {
            var playerPointsEvents =
                await _pointsClient.GetPointsRecord(pointsEvent.Source, pointsEvent.TargetPlayerId);

            playerPointsEvents.Add(pointsEvent);
            await _pointsClient.UpdatePlayer(pointsEvent.Source, pointsEvent.TargetPlayerId, playerPointsEvents);
        }
    }
}