using StackExchange.Redis;

namespace LibraryApi.BusinessLogic.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        public async Task<long> KeyDeleteAsync(string[] keys)
        {
            var redisKeys = Array.ConvertAll(keys, k => (RedisKey)k);
            return await _db.KeyDeleteAsync(redisKeys);
        }
    }
}
