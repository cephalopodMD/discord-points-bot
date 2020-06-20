using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace PointsBot.Core
{
    public interface ICosmosTimerClient
    {
        Task<ResponseMessage> GetPlayerTimeoutRecord(string source, string playerId);
        Task CreatePlayerTimeoutRecord(string source, string playerId);
    }
}