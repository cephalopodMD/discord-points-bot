using System.Threading.Tasks;
using Function.Command.Events;

namespace Function.Query
{
    public interface IEventWriter<TEvent>
    {
        Task PushEvents(TEvent pointsEvent);
    }
}