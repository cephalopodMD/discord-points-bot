using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Function.Query.Redis
{
    public class RedisTimer : IGameTimer
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        private static string TimeoutKey(string playerId) => $"points_{playerId}_timeout";
        public RedisTimer(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _redis = redis;
            _configuration = configuration;
        }

        public bool HasTimeout(string playerId)
        {
            var database = _redis.GetDatabase();

            var timeoutKey = TimeoutKey(playerId);
            return database.StringGet(timeoutKey) != RedisValue.Null;
        }

        public Task Timeout(string playerId)
        {
            var database = _redis.GetDatabase();

            return database.StringSetAsync(TimeoutKey(playerId), new RedisValue("--expirykey--"),
                TimeSpan.FromSeconds(Double.Parse(_configuration["CommandTimeoutInSeconds"])));
        }
    }
}