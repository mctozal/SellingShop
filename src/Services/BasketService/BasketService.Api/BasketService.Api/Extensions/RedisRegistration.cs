using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace BasketService.Extensions
{
    public static class RedisRegistration
    {
        public static ConnectionMultiplexer ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Redis:ConnectionString");

            var redisConfiguration = ConfigurationOptions.Parse(connectionString, true);
            redisConfiguration.ResolveDns = true;

            return ConnectionMultiplexer.Connect(redisConfiguration);

        }

        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ConnectionMultiplexer>(ConfigureRedis(services, configuration));
        }
    }
}
