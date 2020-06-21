using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Function.Commands
{
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