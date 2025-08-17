using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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

        if (databaseSettings.IsEnabled)
        {
            services
                .Configure<RedisDatabaseSettings>(options =>
                    configuration.GetSection(nameof(RedisDatabaseSettings)).Bind(options)
                )
                .AddSingleton(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<RedisDatabaseSettings>>().Value;
                    ConfigurationOptions options =
                        new()
                        {
                            EndPoints = { { settings.Host!, settings.Port!.Value } },
                            Password = settings.Password,
                        };
                    ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(options);
                    return multiplexer.GetDatabase();
                })
                .AddSingleton<IDistributedCacheService, RedisCacheService>();
        }

        return services;
    }
}
