using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using PointsBot.Core;

namespace PointsBot.Infrastructure
{
    public class CosmosEventWriter : IEventWriter<PointsEvent>
    {
        private readonly CosmosClient _client;
        private const string DatabaseName = "points_bot";
        private const string ContainerName = "points_events_monitored";

        public CosmosEventWriter(CosmosClient client)
        {
            _client = client;
        }

        public Task PushEvents(PointsEvent pointsEvent)
        {
            var database = _client.GetDatabase(DatabaseName);
            var container = database.GetContainer(ContainerName);

            return container.CreateItemAsync(new CosmosItem
            {
                id = Guid.NewGuid().ToString(),
                source = pointsEvent.Source,
                Event = pointsEvent
            });
        }

        private class CosmosItem
        {
            public string id { get; set; }

            public string source { get; set; }

            public PointsEvent Event { get; set; }
        }
    }
}