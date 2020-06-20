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
        private readonly Func<Container> _containerFactory;
        public CosmosEventWriter(Func<Container> containerFactory) { _containerFactory = containerFactory; }

        public async Task PushEvents(PointsEvent pointsEvent)
        {
            var container = _containerFactory();

            var playerRecordResponse = await container.ReadItemStreamAsync($"{pointsEvent.Source}_{pointsEvent.TargetPlayerId}",
                new PartitionKey(pointsEvent.Source));

            var newPlayer = playerRecordResponse.StatusCode == HttpStatusCode.NotFound
                ? new CosmosItem
                {
                    id = $"{pointsEvent.Source}_{pointsEvent.TargetPlayerId}",
                    source = pointsEvent.Source,
                    Events = new List<PointsEvent>()
                }
                : await JsonSerializer.DeserializeAsync<CosmosItem>(playerRecordResponse.Content);

            newPlayer.Events.Add(pointsEvent);
            await container.UpsertItemAsync(newPlayer);
        }

        private class CosmosItem
        {
            public string id { get; set; }

            public string source { get; set; }

            public ICollection<PointsEvent> Events { get; set; }
        }
    }
}