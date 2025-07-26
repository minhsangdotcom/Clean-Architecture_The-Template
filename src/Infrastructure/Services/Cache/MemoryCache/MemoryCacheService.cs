using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Cache.MemoryCache;

public class MemoryCacheService(
    IMemoryCache cache,
    IOptions<CacheSettings> options,
    ILogger<MemoryCacheService> logger
) : IMemoryCacheService
{
    private readonly CacheSettings cacheSettings = options.Value;

    public T? GetOrSet<T>(string key, Func<T> func, CacheOptions? options = null)
    {
        return GetOrSetDefault(
            key,
            func,
            options
                ?? new CacheOptions()
                {
                    ExpirationType = CacheExpirationType.Absolute,
                    Expiration = TimeSpan.FromMinutes(cacheSettings.CachingExpirationInMinute),
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
                    Expiration = TimeSpan.FromMinutes(cacheSettings.CachingExpirationInMinute),
                }
        );
    }

    public bool HasKey(string key)
    {
        return cache.TryGetValue(key, out _);
    }

    public void Remove(string key)
    {
        cache.Remove(key);
        logger.LogDebug("Redis KeyDelete {Key}", key);
    }

    private Task<T?> GetOrSetDefaultAsync<T>(string key, Func<Task<T>> task, CacheOptions options)
    {
        var entryOptions = new MemoryCacheEntryOptions();
        if (options.ExpirationType == CacheExpirationType.Absolute && options.Expiration.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(options.Expiration.Value);
        }

        if (options.ExpirationType == CacheExpirationType.Sliding && options.Expiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(options.Expiration.Value);
        }

        return cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(entryOptions);
                logger.LogWarning("fetching source for {key}", key);
                return task();
            }
        );
    }

    private T? GetOrSetDefault<T>(string key, Func<T> func, CacheOptions options)
    {
        var entryOptions = new MemoryCacheEntryOptions();
        if (options.ExpirationType == CacheExpirationType.Absolute && options.Expiration.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(options.Expiration.Value);
        }

        if (options.ExpirationType == CacheExpirationType.Sliding && options.Expiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(options.Expiration.Value);
        }

        return cache.GetOrCreate(
            key,
            entry =>
            {
                entry.SetOptions(entryOptions);
                logger.LogWarning("fetching source for {key}", key);
                return func();
            }
        );
    }
}
