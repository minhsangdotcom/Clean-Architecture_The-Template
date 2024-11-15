using Application.Common.Interfaces.Registers;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IQueueService : ISingleton
{
    public long Size { get; }

    public long Length();

    public Task<bool> EnqueueAsync<T>(T payload);

    public Task<T?> DequeueAsync<T>();
}
