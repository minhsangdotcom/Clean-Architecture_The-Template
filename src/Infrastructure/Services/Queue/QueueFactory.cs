using Application.Common.Interfaces.Services.Queue;
using Domain.Aggregates.QueueLogs;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Queue;

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
