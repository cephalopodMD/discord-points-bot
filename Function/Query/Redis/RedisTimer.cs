using StackExchange.Redis;

namespace Function.Query.Redis
{
    public class RedisTimer : IGameTimer
    {
        private readonly IConnectionMultiplexer _redis;
        private static string TimeoutKey(string playerId) => $"points_{playerId}_timeout";
        public RedisTimer(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public bool HasTimeout(string playerId)
        {
            var database = _redis.GetDatabase();

            var timeoutKey = TimeoutKey(playerId);
            return database.StringGet(timeoutKey) != RedisValue.Null;
        }
    }
}