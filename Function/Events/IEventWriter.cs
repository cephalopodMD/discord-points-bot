using System.Threading.Tasks;

namespace Function.Events
{
    public interface IEventWriter<TEvent>
    {
        Task PushEvents(TEvent pointsEvent);
    }
}