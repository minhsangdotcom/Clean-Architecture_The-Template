using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.Cache.DistributedCache;

public class RedisCacheService(
    IDatabase redis,
    IOptions<RedisDatabaseSettings> options,
    ILogger<RedisCacheService> logger
) : IDistributedCacheService
{
    private readonly RedisDatabaseSettings redisDatabaseSettings = options.Value;

    public T? GetOrSet<T>(string key, Func<T> func, CacheOptions? options = null)
    {
        return GetOrSetDefault(
            key,
            func,
            options
                ?? new CacheOptions()
                {
                    ExpirationType = CacheExpirationType.Absolute,
                    Expiration = TimeSpan.FromMinutes(
                        redisDatabaseSettings.DefaultCachingTimeInMinute
                    ),
                }
        );
    }

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> task,
        CacheOptions? options = null
    )
    {
        return await GetOrSetDefaultAsync(
            key,
            task,
            options
                ?? new CacheOptions()
                {
                    ExpirationType = CacheExpirationType.Absolute,
                    Expiration = TimeSpan.FromMinutes(
                        redisDatabaseSettings.DefaultCachingTimeInMinute
                    ),
                }
        );
    }

    public void Remove(string key)
    {
        bool isSuccess = redis.KeyDelete(key);
        if (isSuccess)
        {
            logger.LogDebug("Redis KeyDelete {Key}", key);
        }
        else
        {
            logger.LogDebug("Redis KeyDelete {Key} failed", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        bool isSuccess = await redis.KeyDeleteAsync(key);

        if (isSuccess)
        {
            logger.LogDebug("Redis KeyDelete {Key}", key);
        }
        else
        {
            logger.LogDebug("Redis KeyDelete {Key} failed", key);
        }
    }

    private async Task<T?> GetOrSetDefaultAsync<T>(
        string key,
        Func<Task<T>> task,
        CacheOptions options
    )
    {
        RedisValue redisValue = await redis.StringGetAsync(key);
        if (redisValue.HasValue)
        {
            logger.LogWarning("Redis HIT for {Key}", key);
            if (
                options.ExpirationType == CacheExpirationType.Sliding
                && options.Expiration.HasValue
            )
            {
                await redis.KeyExpireAsync(key, options.Expiration);
            }

            return SerializerExtension.Deserialize<T>(redisValue!).Object;
        }

        logger.LogWarning("Redis MISS for {Key}, invoking task", key);
        T result = await task();
        string json = SerializerExtension.Serialize(result!).StringJson;

        TimeSpan? expiry = options.ExpirationType switch
        {
            CacheExpirationType.Absolute => options.Expiration
                ?? TimeSpan.FromMinutes(redisDatabaseSettings.DefaultCachingTimeInMinute),
            CacheExpirationType.Sliding => options.Expiration
                ?? TimeSpan.FromMinutes(redisDatabaseSettings.DefaultCachingTimeInMinute),
            _ => null,
        };

        await redis.StringSetAsync(key, json, expiry);
        return result;
    }

    private T? GetOrSetDefault<T>(string key, Func<T> func, CacheOptions options)
    {
        RedisValue redisValue = redis.StringGet(key);
        if (redisValue.HasValue)
        {
            logger.LogWarning("Redis HIT for {Key}", key);
            if (
                options.ExpirationType == CacheExpirationType.Sliding
                && options.Expiration.HasValue
            )
            {
                redis.KeyExpire(key, options.Expiration);
            }

            return SerializerExtension.Deserialize<T>(redisValue!).Object;
        }

        logger.LogWarning("Redis MISS for {Key}, invoking func", key);
        T result = func();
        string json = SerializerExtension.Serialize(result!).StringJson;

        TimeSpan? expiry = options.ExpirationType switch
        {
            CacheExpirationType.Absolute => options.Expiration
                ?? TimeSpan.FromMinutes(redisDatabaseSettings.DefaultCachingTimeInMinute),
            CacheExpirationType.Sliding => options.Expiration
                ?? TimeSpan.FromMinutes(redisDatabaseSettings.DefaultCachingTimeInMinute),
            _ => null,
        };

        redis.StringSet(key, json, expiry);
        return result;
    }
}
