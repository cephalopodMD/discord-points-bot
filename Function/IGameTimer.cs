using System.Threading.Tasks;

namespace Function
{
    public interface IGameTimer
    {
        Task<bool> HasTimeout(string playerId, string source);

        Task Timeout(string playerId, string source);
    }
}