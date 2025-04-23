using Application.Common.Interfaces.Services.Queue;
using Infrastructure.Services.Cache.DistributedCache;
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
                .Configure<HostOptions>(options =>
                {
                    options.ServicesStartConcurrently = true;
                    options.ServicesStopConcurrently = true;
                })
                .AddHostedService<QueueBackgroundService>()
                .AddSingleton<IQueueService, QueueService>();
        }

        return services;
    }
}
