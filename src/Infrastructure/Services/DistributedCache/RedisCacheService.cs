using Application.Common.Interfaces.Services.DistributedCache;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services.DistributedCache;

public class RedisCacheService(IOptions<RedisDatabaseSettings> options) : IRedisCacheService
{
    private readonly RedisDatabaseSettings redisDatabaseSettings = options.Value;
    public IDatabase Database => GetDatabase();

    private IDatabase GetDatabase()
    {
        ConfigurationOptions options =
            new()
            {
                EndPoints = { { redisDatabaseSettings.Host!, redisDatabaseSettings.Port! } },
                Password = redisDatabaseSettings.Password,
            };
        ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(options);

        return multiplexer.GetDatabase();
    }
}
