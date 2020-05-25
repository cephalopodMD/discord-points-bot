using System.Threading.Tasks;

namespace Function.Query
{
    public interface IGameTimer
    {
        bool HasTimeout(string playerId);

        Task Timeout(string playerId);
    }
}