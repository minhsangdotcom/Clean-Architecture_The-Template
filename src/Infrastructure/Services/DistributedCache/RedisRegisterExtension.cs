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
        RedisDatabaseSettings? databaseSettings = configuration
            .GetSection(nameof(RedisDatabaseSettings))
            .Get<RedisDatabaseSettings>();

        services.Configure<RedisDatabaseSettings>(options =>
            configuration.GetSection(nameof(RedisDatabaseSettings)).Bind(options)
        );

        services.AddHostedService<QueueBackgroundService>();

        return services;
    }
}
