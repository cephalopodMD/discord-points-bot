using System.Threading.Tasks;

namespace PointsBot.Core
{
    public interface IEventWriter<TEvent>
    {
        Task PushEvents(TEvent pointsEvent);
    }
}