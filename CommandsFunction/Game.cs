using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CommandsFunction
{
    public class Game : IGame
    {
        private IList<Player> _players { get; }

        public Game(string gameState)
        {
            _players = JsonSerializer.Deserialize<IList<Player>>(gameState);
        }

        public IEnumerable<Player> GetPlayers() => _players;
        public Player GetPlayer(string playerId) => _players.SingleOrDefault(player => player.Id == playerId);

        public bool HasPlayer(string playerId) => _players.Any(p => p.Id == playerId);

        public void AddPlayer(string playerId)
        {
            if (HasPlayer(playerId)) return;

            var newPlayer = new Player {Id = playerId, TotalPoints = 0};
            _players.Add(newPlayer);
        }

        public string Serialize() => JsonSerializer.Serialize(_players);
    }
}