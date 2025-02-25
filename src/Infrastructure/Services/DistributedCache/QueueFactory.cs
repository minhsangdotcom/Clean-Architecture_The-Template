using Application.Common.Interfaces.Registers;
using Application.Common.Interfaces.Services.DistributedCache;
using Domain.Aggregates.QueueLogs;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.DistributedCache;

public class QueueFactory(IServiceScopeFactory serviceScopeFactory) : IQueueFactory
{
    public IQueueService GetQueue(QueueType type)
    {
        using var scope = serviceScopeFactory.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        return type switch
        {
            QueueType.OriginQueue => serviceProvider.GetRequiredService<QueueService>(),
            QueueType.DeadLetterQueue =>
                serviceProvider.GetRequiredService<DeadLetterQueueService>(),
            _ => throw new ArgumentException("Invalid email provider"),
        };
    }
}
