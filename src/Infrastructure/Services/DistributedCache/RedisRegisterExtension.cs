using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.DistributedCache;

public static class RedisRegisterExtension
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        RedisDatabaseSettings databaseSettings =
            configuration.GetSection(nameof(RedisDatabaseSettings)).Get<RedisDatabaseSettings>()
            ?? new();

        if (databaseSettings.IsEnbaled)
        {
            services
                .Configure<RedisDatabaseSettings>(options =>
                    configuration.GetSection(nameof(RedisDatabaseSettings)).Bind(options)
                )
                .AddSingleton<IRedisCacheService, RedisCacheService>();
        }

        return services;
    }
}
