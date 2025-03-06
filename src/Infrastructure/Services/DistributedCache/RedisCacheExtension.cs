using Application.Common.Interfaces.Services.DistributedCache;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.DistributedCache;

public static class RedisCacheExtension
{
    public static async Task<string> GetOrSetAsync<T>(
        this IRedisCacheService cache,
        string key,
        Func<Task<T>> task,
        TimeSpan expiry
    )
    {
        string? currentValue = await cache.Database.StringGetAsync(key);

        if (currentValue == null)
        {
            T value = await task();
            var result = SerializerExtension.Serialize(value!);
            _ = await cache.Database.StringSetAsync(
                key,
                result.StringJson,
                expiry,
                when: When.Always
            );

            return result.StringJson;
        }

        return currentValue;
    }
}
