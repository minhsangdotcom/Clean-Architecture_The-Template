using Application.Common.Interfaces.Services.DistributedCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .Configure<QueueSettings>(options =>
                    configuration.GetSection(nameof(QueueSettings)).Bind(options)
                )
                .AddSingleton<QueueService>()
                .AddSingleton<IQueueService, QueueService>(provider =>
                    provider.GetService<QueueService>()!
                )
                .AddSingleton<DeadLetterQueueService>()
                .AddSingleton<IQueueService, DeadLetterQueueService>(provider =>
                    provider.GetService<DeadLetterQueueService>()!
                );

            services
                .Configure<HostOptions>(options =>
                {
                    options.ServicesStartConcurrently = true;
                    options.ServicesStopConcurrently = true;
                })
                .AddHostedService<QueueBackgroundService>()
                .AddHostedService<DeadletterQueueBackgroundService>()
                .AddSingleton<IRedisCacheService, RedisCacheService>()
                .AddSingleton<IQueueService, QueueService>()
                .AddSingleton<IQueueService, DeadLetterQueueService>();
        }

        return services;
    }
}
