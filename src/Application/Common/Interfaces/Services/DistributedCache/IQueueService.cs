using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Requests;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IQueueService : ISingleton
{
    public long Size { get; }

    public long Length();

    public Task<bool> EnqueueAsync<T>(T payload);

    public Task<QueueRequest<T>?> DequeueAsync<T>();
}
