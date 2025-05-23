using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.Services.Queue;
using Contracts.Dtos.Requests;
using Microsoft.Extensions.Options;
using NRedisStack;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.Queue;

public class QueueService(IDistributedCacheService redisCache, IOptions<QueueSettings> options)
    : IQueueService
{
    private readonly QueueSettings queueSettings = options.Value;

    public long Size => size;

    private long size;

    /// <summary>
    /// Get queue from redis
    /// </summary>
    /// <typeparam name="TResponse">Map to the response</typeparam>
    /// <typeparam name="TRequest">Type of the input that putting in queue before, treat as part of queue name</typeparam>
    /// <returns></returns>
    public async Task<TResponse?> DequeueAsync<TResponse, TRequest>()
    {
        string queueName = $"{queueSettings.OriginQueueName}:{typeof(TRequest).Name}";
        Tuple<RedisKey, RedisValue>? value = await redisCache.Database.BRPopAsync([queueName], 1);

        if (value == null)
        {
            return default;
        }

        var result = SerializerExtension.Deserialize<TResponse>(value.Item2.ToString());
        size = Length();
        return result.Object!;
    }

    /// <summary>
    /// Add request in queue
    /// </summary>
    /// <typeparam name="T"> type of the request, treat as part of queue name</typeparam>
    /// <param name="payload"></param>
    /// <returns></returns>
    public async Task<bool> EnqueueAsync<T>(T payload)
    {
        QueueRequest<T> request = new() { PayloadId = Guid.NewGuid(), Payload = payload };
        var result = SerializerExtension.Serialize(request);
        string queueName = $"{queueSettings.OriginQueueName}:{typeof(T).Name}";
        long length = await redisCache.Database.ListLeftPushAsync(queueName, result.StringJson);
        size = length;

        return length > 0;
    }

    public long Length() => redisCache.Database.ListLength(queueSettings.OriginQueueName);

    public async Task<bool> PingAsync()
    {
        try
        {
            var result = await redisCache.Database.PingAsync();

            return result.TotalMilliseconds > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
