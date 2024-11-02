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
    private long size = 0;

    public async Task<QueueRequest<T>?> DequeueAsync<T>()
    {
        Tuple<RedisKey, RedisValue>? value = await redisCache.Database.BRPopAsync([QUEUE_NAME], 1);

        if (value == null)
        {
            return null;
        }

        var result = SerializerExtension.Deserialize<QueueRequest<T>?>(value.Item2!);
        size = Length();
        return result.Object;
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
