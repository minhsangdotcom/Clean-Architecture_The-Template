using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.Cache.DistributedCache;

public class RedisCacheService(
    IOptions<RedisDatabaseSettings> options,
    ILogger<RedisCacheService> logger
) : IDistributedCacheService
{
    private readonly RedisDatabaseSettings redisDatabaseSettings = options.Value;

    public IDatabase Database => GetDatabase();

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
        bool isSuccess = Database.KeyDelete(key);
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
        bool isSuccess = await Database.KeyDeleteAsync(key);

        if (isSuccess)
        {
            logger.LogDebug("Redis KeyDelete {Key}", key);
        }
        else
        {
            logger.LogDebug("Redis KeyDelete {Key} failed", key);
        }
    }

    private IDatabase GetDatabase()
    {
        ConfigurationOptions options =
            new()
            {
                EndPoints = { { redisDatabaseSettings.Host!, redisDatabaseSettings.Port!.Value } },
                Password = redisDatabaseSettings.Password,
            };
        ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(options);

        return multiplexer.GetDatabase();
    }

    private async Task<T?> GetOrSetDefaultAsync<T>(
        string key,
        Func<Task<T>> task,
        CacheOptions options
    )
    {
        RedisValue redisValue = await Database.StringGetAsync(key);
        if (redisValue.HasValue)
        {
            logger.LogWarning("Redis HIT for {Key}", key);
            if (
                options.ExpirationType == CacheExpirationType.Sliding
                && options.Expiration.HasValue
            )
            {
                await Database.KeyExpireAsync(key, options.Expiration);
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

        await Database.StringSetAsync(key, json, expiry);
        return result;
    }

    private T? GetOrSetDefault<T>(string key, Func<T> func, CacheOptions options)
    {
        RedisValue redisValue = Database.StringGet(key);
        if (redisValue.HasValue)
        {
            logger.LogWarning("Redis HIT for {Key}", key);
            if (
                options.ExpirationType == CacheExpirationType.Sliding
                && options.Expiration.HasValue
            )
            {
                Database.KeyExpire(key, options.Expiration);
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

        Database.StringSet(key, json, expiry);
        return result;
    }
}
