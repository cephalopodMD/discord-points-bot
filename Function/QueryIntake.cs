using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PointsBot.Core.Models;

namespace Function
{
    public class QueryIntake
    {
        private readonly GameState _gameState;
        public QueryIntake(GameState gameState)
        {
            _gameState = gameState;
        }

        private static readonly Dictionary<string, PlayerState> PlayerStateByRedisKey =
            new Dictionary<string, PlayerState>();

        [FunctionName("GetPlayerPoints")]
        public Task<PlayerState> GetPlayerPoints(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "points/{playerId}")]
            HttpRequest request, string playerId)
        {
            return _gameState.RefreshPlayer(playerId);
        }

        [FunctionName("GetTimeout")]
        public async Task<object> GetPlayerTimeout(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "timeout/{playerId}")]
            HttpRequest request, string playerId)
        {
            return _gameState.IsPlayerTimedOut(playerId);
        }
    }
}