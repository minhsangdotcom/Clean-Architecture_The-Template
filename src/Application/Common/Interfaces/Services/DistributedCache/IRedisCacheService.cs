using StackExchange.Redis;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IRedisCacheService
{
    IDatabase Database { get; }
}
