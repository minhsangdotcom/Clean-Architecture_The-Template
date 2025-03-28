using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.Services.Queue;
using Microsoft.Extensions.Options;
using NRedisStack;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.Queue;

public class DeadLetterQueueService(IRedisCacheService redisCache, IOptions<QueueSettings> options)
    : IQueueService
{
    private readonly QueueSettings queueSettings = options.Value;

    public long Size => size;

    private long size;

    public async Task<TResponse?> DequeueAsync<TResponse, TRequest>()
    {
        string queueName = $"{queueSettings.DeadLetterQueueName}:{typeof(TRequest).Name}";
        Tuple<RedisKey, RedisValue>? value = await redisCache.Database.BRPopAsync([queueName], 1);

        if (value == null)
        {
            return default;
        }

        var result = SerializerExtension.Deserialize<TResponse>(value.Item2.ToString());
        size = Length();
        return result.Object!;
    }

    public async Task<bool> EnqueueAsync<T>(T payload)
    {
        var result = SerializerExtension.Serialize(payload!);
        string queueName = $"{queueSettings.DeadLetterQueueName}:{typeof(T).Name}";
        long length = await redisCache.Database.ListLeftPushAsync(queueName, result.StringJson);
        size = length;

        return length > 0;
    }

    public long Length() => redisCache.Database.ListLength(queueSettings.DeadLetterQueueName);

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
