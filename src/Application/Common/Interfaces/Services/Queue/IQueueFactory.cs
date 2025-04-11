using Domain.Aggregates.QueueLogs;

namespace Application.Common.Interfaces.Services.Queue;

public interface IQueueFactory
{
    IQueueService GetQueue(QueueType type);
}
