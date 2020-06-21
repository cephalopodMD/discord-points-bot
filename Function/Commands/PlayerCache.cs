using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage.Table;

namespace Function.Commands
{
    public class PlayerCache
    {
        private readonly IMemoryCache _cache;
        private readonly PlayerStorage _playerStorage;

        public PlayerCache(IMemoryCache cache, PlayerStorage playerStorage)
        {
            _cache = cache;
            _playerStorage = playerStorage;
        }

        public async Task<PlayerPoints> GetPlayer(string playerId)
        {
            return await _cache.GetOrCreateAsync($"{playerId}", async entry =>
            {
                var player = await _playerStorage.GetPlayer(playerId);
                return player;
            });
        }

        public void UpdatePlayer(IEnumerable<PlayerPoints> playerPoints) { foreach (var player in playerPoints) _cache.Set($"{player.Source}_{player.PlayerName}", player); }
    }
}