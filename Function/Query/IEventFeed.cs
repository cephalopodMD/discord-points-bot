using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Query
{
    public interface IEventFeed<TEvent>
    {
        Task<IEnumerable<TEvent>> GetEvents(string playerId);
    }
}