using System.Threading.Tasks;

namespace Function
{
    public interface IGameTimer
    {
        bool HasTimeout(string playerId);

        Task Timeout(string playerId);
    }
}