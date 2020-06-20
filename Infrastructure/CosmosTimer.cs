using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PointsBot.Core;

namespace PointsBot.Infrastructure
{
    public class CosmosTimer : IGameTimer
    {
        private readonly CosmosClient _client;
        private readonly TimeoutOptions _timeout;

        public CosmosTimer(CosmosClient client, IOptions<TimeoutOptions> timeout)
        {
            _client = client;
            _timeout = timeout.Value;
        }

        public async Task<bool> HasTimeout(string playerId, string source)
        {
            var container = _client.GetContainer(_timeout.DatabaseName, _timeout.ContainerName);

            var timeoutRecordResponse =
                await container.ReadItemStreamAsync($"{source}_{playerId}", new PartitionKey(source));

            if (timeoutRecordResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception($"Error occured trying to timeout for player ({playerId})");

            return timeoutRecordResponse.StatusCode != HttpStatusCode.NotFound;
        }

        public async Task Timeout(string playerId, string source)
        {
            var container = _client.GetContainer(_timeout.DatabaseName, _timeout.ContainerName);

            var existingItem = await container.ReadItemStreamAsync($"{source}_{playerId}", new PartitionKey(source));
            if (existingItem.StatusCode == HttpStatusCode.InternalServerError) throw new Exception($"Error occured when attempting to see if player ({playerId}) is timed out");
            if (existingItem.StatusCode == HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(new TimeoutRecord
                    { source = source, PlayerId = playerId, ttl = _timeout.InSeconds });
            }
        }

        private class TimeoutRecord
        {
            public string id => $"{source}_{PlayerId}";

            public string source { get; set; }

            [JsonPropertyName("playerId")]
            public string PlayerId { get; set; }

            public int ttl { get; set; }
        }
    }
}