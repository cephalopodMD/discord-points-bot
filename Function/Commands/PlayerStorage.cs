using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Function.Commands
{
    public class PlayerStorage
    {
        private readonly PointsStorage _pointsStorage;

        public PlayerStorage(PointsStorage pointsStorage)
        {
            _pointsStorage = pointsStorage;
        }

        public async Task<PlayerPoints> GetPlayer(string playerId)
        {
            var playerResult = await _pointsStorage.GetPlayer(playerId);

            return playerResult.HttpStatusCode == 404
                ? await AddPlayer(playerId)
                : (PlayerPoints)playerResult.Result;
        }

        private async Task<PlayerPoints> AddPlayer(string playerId)
        {
            var newPlayer = await _pointsStorage.AddPlayer(playerId);
            return (PlayerPoints) newPlayer.Result;
        }

        public Task UpdatePlayer(PlayerPoints playerPoints) => _pointsStorage.UpdatePlayer(playerPoints);
    }
}