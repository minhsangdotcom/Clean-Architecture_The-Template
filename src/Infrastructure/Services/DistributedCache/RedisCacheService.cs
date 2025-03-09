using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Options;
using SharedKernel.Extensions;
using StackExchange.Redis;

namespace Infrastructure.Services.DistributedCache;

public class RedisCacheService(IOptions<RedisDatabaseSettings> options) : IRedisCacheService
{
    private readonly RedisDatabaseSettings redisDatabaseSettings = options.Value;

    public IDatabase Database => GetDatabase();

    public T? GetOrSet<T>(string key, Func<T> func, TimeSpan? expiry = null)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> task, TimeSpan? expiry = null)
    {
        string? currentValue = await Database.StringGetAsync(key);

        if (currentValue == null)
        {
            T value = await task();
            SerializeResult result = SerializerExtension.Serialize(value!);
            _ = await Database.StringSetAsync(key, result.StringJson, expiry, when: When.Always);
            return value;
        }

        return SerializerExtension.Deserialize<T>(currentValue).Object;
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
}
