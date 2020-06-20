using System.Threading.Tasks;

namespace PointsBot.Core
{
    public interface IGameTimer
    {
        Task<bool> HasTimeout(string playerId, string source);

        Task Timeout(string playerId, string source);
    }
}