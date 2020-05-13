using System.Collections.Generic;

namespace CommandsFunction
{
    public interface IGame
    {
        IEnumerable<Player> GetPlayers();
        Player GetPlayer(string playerId);
        void AddPlayer(string playerId);
        bool HasPlayer(string playerId);
    }
}