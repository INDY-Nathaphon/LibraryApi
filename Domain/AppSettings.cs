using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace LibraryApi.Domain
{
    public class AppSettings
    {
        public ConnectionStringSettings ConnectionSetting { get; set; } = new();
        public JwtSettings JwtSetting { get; set; } = new();
        public RedisSettings RedisSetting { get; set; } = new();

        public class ConnectionStringSettings
        {
            public string DbConnection { get; set; } = string.Empty;
        }

        public class JwtSettings
        {
            public string Secret { get; set; } = string.Empty;
            public string Issuer { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
            public int ExpiresInMinutes { get; set; }
        }

        public class RedisSettings
        {
            public string ConnectionString { get; set; } = string.Empty;
            public int TokenExpiryMinutes { get; set; }
        }
    }

    // Extension method for binding configuration
    public static class ServiceExtensions
    {
        public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
        }
    }
}
