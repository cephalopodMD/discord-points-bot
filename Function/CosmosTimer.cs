using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Function
{
    public class CosmosTimer : IGameTimer
    {
        private readonly CosmosClient _client;
        private readonly IConfiguration _configuration;

        public CosmosTimer(CosmosClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<bool> HasTimeout(string playerId, string source)
        {
            var container = _client.GetContainer(_configuration["CosmosDatabaseName"],
                _configuration["TimeoutContainerName"]);

            var timeoutRecordResponse =
                await container.ReadItemStreamAsync($"{source}_{playerId}", new PartitionKey(source));

            if (timeoutRecordResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception($"Error occured trying to timeout for player ({playerId})");

            return timeoutRecordResponse.StatusCode != HttpStatusCode.NotFound;
        }

        public async Task Timeout(string playerId, string source)
        {
            var container = _client.GetContainer(_configuration["CosmosDatabaseName"],
                _configuration["TimeoutContainerName"]);

            var existingItem = await container.ReadItemStreamAsync($"{source}_{playerId}", new PartitionKey(source));
            if (existingItem.StatusCode == HttpStatusCode.InternalServerError) throw new Exception($"Error occured when attempting to see if player ({playerId}) is timed out");
            if (existingItem.StatusCode == HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(new TimeoutRecord
                    { source = source, PlayerId = playerId, ttl = Int32.Parse(_configuration["CommandTimeoutInSeconds"]) });
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