namespace LibraryApi.BusinessLogic.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task<bool> KeyDeleteAsync(string key);
        Task<long> KeyDeleteAsync(string[] keys); // สำหรับลบหลาย Key
    }
}
