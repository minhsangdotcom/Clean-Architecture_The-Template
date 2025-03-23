using Application.Common.Interfaces.Registers;
using Domain.Aggregates.QueueLogs;

namespace Application.Common.Interfaces.Services.Queue;

public interface IQueueFactory : ISingleton
{
    IQueueService GetQueue(QueueType type);
}
