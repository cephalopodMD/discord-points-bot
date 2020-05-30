using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointsBot.Core
{
    public interface IEventFeed<TEvent>
    {
        Task<IEnumerable<TEvent>> GetEvents(string playerId);
    }
}