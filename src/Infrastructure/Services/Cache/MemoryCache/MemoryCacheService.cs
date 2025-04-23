using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Services.Cache.MemoryCache;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheOptions;
    private readonly CacheSettings cacheSettings;
    private readonly ILogger logger;

    public MemoryCacheService(IMemoryCache cache, IOptions<CacheSettings> options, ILogger logger)
    {
        this.cache = cache;
        cacheSettings = options.Value;
        cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            relative: TimeSpan.FromMinutes(cacheSettings.RepositoryCachingTimeInMinute)
        );
        this.logger = logger;
    }

    public T? GetOrSet<T>(string key, Func<T> func, TimeSpan? expiry = null)
    {
        return cache.GetOrCreate(
            key,
            entry =>
            {
                entry.SetOptions(cacheOptions);
                logger.Warning("fetching source for {key}", key);
                return func();
            }
        );
    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> task, TimeSpan? expiry = null)
    {
        return cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(cacheOptions);
                logger.Warning("fetching source for {key}", key);
                return task();
            }
        );
    }
}
