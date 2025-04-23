using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Cache.DistributedCache;

public static class DistributedCacheExtension
{
    public static IServiceCollection AddDistributedCache(
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
                .AddSingleton<IDistributedCacheService, RedisCacheService>();
        }

        return services;
    }
}
