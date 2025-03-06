namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IQueueService
{
    public long Size { get; }

    public long Length();

    public Task<bool> EnqueueAsync<T>(T payload);

    public Task<TResponse?> DequeueAsync<TResponse, TRequest>();

    public Task<bool> PingAsync();
}
