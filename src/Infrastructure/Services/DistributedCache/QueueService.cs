using Application.Common.Interfaces.Services.DistributedCache;
using Contracts.Dtos.Requests;
using Contracts.Extensions;
using NRedisStack;
using StackExchange.Redis;

namespace Infrastructure.Services.DistributedCache;

public class QueueService(IRedisCacheService redisCache) : IQueueService
{
    private const string QUEUE_NAME = "the_queue";

    public long Size => size;

    private long size;

    public async Task<T?> DequeueAsync<T>()
    {
        Tuple<RedisKey, RedisValue>? value = await redisCache.Database.BRPopAsync([QUEUE_NAME], 1);

        if (value == null)
        {
            return default;
        }

        var result = SerializerExtension.Deserialize<T>(value.Item2.ToString());
        size = Length();
        return result.Object!;
    }

    public async Task<bool> EnqueueAsync<T>(T payload)
    {
        QueueRequest<T> request = new() { PayloadId = Guid.NewGuid(), Payload = payload };
        var result = SerializerExtension.Serialize(request);
        long length = await redisCache.Database.ListLeftPushAsync(QUEUE_NAME, result.StringJson);
        size = length;

        return length > 0;
    }

    public long Length() => redisCache.Database.ListLength(QUEUE_NAME);
}
