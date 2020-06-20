using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointsBot.Core
{
    public interface ICosmosPointsClient
    {
        Task<ICollection<PointsEvent>> GetPointsRecord(string source, string playerId);
        Task UpdatePlayer(string source, string playerId, ICollection<PointsEvent> newEvents);
    }
}