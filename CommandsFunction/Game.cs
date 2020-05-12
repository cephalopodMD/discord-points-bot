using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CommandsFunction
{
    public class Game : IGame
    {
        public IList<Player> Players { get; }

        public Game(string gameState)
        {
            Players = JsonSerializer.Deserialize<IList<Player>>(gameState);
        }

        public Player GetPlayer(string playerId) => Players.SingleOrDefault(player => player.Id == playerId);
        public bool HasPlayer(string playerId) => Players.Any(p => p.Id == playerId);

        public void AddPlayer(string playerId)
        {
            if (HasPlayer(playerId)) return;

            var newPlayer = new Player {Id = playerId, TotalPoints = 0};
            Players.Add(newPlayer);
        }
    }
}