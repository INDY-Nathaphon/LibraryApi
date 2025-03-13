using StackExchange.Redis;

namespace LibraryApi.BusinessLogic.Service.TokenBlacklist
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDatabase _redisDb;
        private readonly IConfiguration _config;

        public TokenBlacklistService(IConfiguration config)
        {
            _config = config;
            var x = _config["Redis:ConnectionString"];
            var redis = ConnectionMultiplexer.Connect(_config["Redis:ConnectionString"]);
            _redisDb = redis.GetDatabase();
        }

        public async Task RevokeTokenAsync(string token, int expiresInMinutes)
        {
            var expiry = TimeSpan.FromMinutes(expiresInMinutes);
            await _redisDb.StringSetAsync(token, "revoked", expiry);
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            return await _redisDb.KeyExistsAsync(token);
        }
    }
}
