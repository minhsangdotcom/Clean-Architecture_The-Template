using Application.Common.Interfaces.Services.Queue;
using Infrastructure.Services.DistributedCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.Queue;

public static class QueueRegisterExtension
{
    public static IServiceCollection AddQueue(
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
                )
                .Configure<HostOptions>(options =>
                {
                    options.ServicesStartConcurrently = true;
                    options.ServicesStopConcurrently = true;
                })
                .AddHostedService<QueueBackgroundService>()
                .AddHostedService<DeadletterQueueBackgroundService>()
                .AddSingleton<IQueueService, QueueService>()
                .AddSingleton<IQueueService, DeadLetterQueueService>();
        }

        return services;
    }
}
