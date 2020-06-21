using System.Net;
using System.Threading.Tasks;
using PointsBot.Core;

namespace PointsBot.Infrastructure
{
    public class CosmosTimer : IGameTimer
    {
        private readonly ICosmosTimerClient _timerClient;

        public CosmosTimer(ICosmosTimerClient timerClient)
        {
            _timerClient = timerClient;
        }

        public async Task<bool> HasTimeout(string playerId, string source)
        {
            var timeoutRecordResponse = await _timerClient.GetPlayerTimeoutRecord(source, playerId);
            return timeoutRecordResponse.StatusCode != HttpStatusCode.NotFound;
        }

        public async Task Timeout(string playerId, string source)
        {
            var existingItem = await _timerClient.GetPlayerTimeoutRecord(source, playerId);
            if (existingItem.StatusCode == HttpStatusCode.NotFound)
            {
                await _timerClient.CreatePlayerTimeoutRecord(source, playerId);
            }
        }
    }
}