using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using PointsBot.Core;

namespace PointsBot.Infrastructure
{
    public class CosmosEventWriter : IEventWriter<PointsEvent>
    {
        public Task PushEvents(PointsEvent pointsEvent)
        {
            var client =
                new CosmosClient(
                    "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

            var database = client.GetDatabase("points_bot");
            var container = database.GetContainer("points_events_monitored");

            return container.CreateItemAsync(new CosmosItem
            {
                id = Guid.NewGuid().ToString(),
                server = "CustomServer",
                Event = pointsEvent
            });
        }

        private class CosmosItem
        {
            public string id { get; set; }

            public string server { get; set; }

            public PointsEvent Event { get; set; }
        }
    }
}