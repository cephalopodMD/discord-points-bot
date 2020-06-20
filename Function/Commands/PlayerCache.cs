using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<PlayerPoints> GetPlayer(CloudTable pointsTable, string playerId)
        {
            return await _cache.GetOrCreateAsync($"{playerId}", async entry =>
            {
                var player = await _playerStorage.GetPlayer(playerId);
                return player;
            });
        }

        public void UpdatePlayer(IEnumerable<PlayerPoints> playerPoints) { foreach (var player in playerPoints) _cache.Set($"{player.Source}_{player.PlayerName}", player); }
    }

    public class PlayerStorage
    {
        private readonly PointsStorage _pointsStorage;

        public PlayerStorage(Func<string, CloudTable> tableFactory, PointsStorage pointsStorage)
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

    public class PointsStorage
    {
        private const string CloudTableName = "points";
        private readonly CloudTable _pointsTable;

        public PointsStorage(Func<string, CloudTable> tableFactory)
        {
            _pointsTable = tableFactory(CloudTableName);
        }

        public async Task<TableResult> GetPlayer(string playerId)
        {
            var (source, player) = ParsePlayerId(playerId);

            var retrieveAction =
                TableOperation.Retrieve<PlayerPoints>(source, player);
            var playerPointsResult = await _pointsTable.ExecuteAsync(retrieveAction);

            if (playerPointsResult.HttpStatusCode >= 500)
            {
                throw new WebException(
                    $"Error getting player (Source: {source}, Player: {player}  : {playerPointsResult.Result} ",
                    WebExceptionStatus.UnknownError);
            }

            return playerPointsResult;
        }

        public async Task<TableResult> AddPlayer(string playerId)
        {
            var (source, player) = ParsePlayerId(playerId);
            var newPlayer = new PlayerPoints
            {
                PartitionKey = source,
                RowKey = player,
                TotalPoints = 0,
                LastEventIndex = -1
            };

            var addAction = TableOperation.Insert(newPlayer);
            var addResult = await _pointsTable.ExecuteAsync(addAction);

            if (addResult.HttpStatusCode >= 500)
            {
                throw new WebException(
                    $"Error adding new player to storage account: {addResult.Result} ",
                    WebExceptionStatus.UnknownError);
            }

            return addResult;
        }

        public async Task<TableResult> UpdatePlayer(PlayerPoints playerPoints)
        {
            var mergeAction = TableOperation.Merge(playerPoints);
            var mergeResult = await _pointsTable.ExecuteAsync(mergeAction);

            if (mergeResult.HttpStatusCode >= 500)
            {
                throw new WebException($"Error updating player (Player: {playerPoints.PlayerName}, Source: {playerPoints.PartitionKey})  : {mergeResult.Result} ",
                    WebExceptionStatus.UnknownError);
            }

            return mergeResult;
        }

        private const char Delimiter = '_';
        private static (string source, string player) ParsePlayerId(string playerId)
        {
            var splitPlayerId = playerId.Split(Delimiter);
            if (splitPlayerId.Length != 3) throw new Exception($"Player Id ({playerId}) is malformed.");

            return ($"{splitPlayerId[0]}_{splitPlayerId[1]}", splitPlayerId[2]);
        }
    }
}