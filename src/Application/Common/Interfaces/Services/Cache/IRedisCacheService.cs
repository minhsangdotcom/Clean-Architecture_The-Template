using StackExchange.Redis;

namespace Application.Common.Interfaces.Services.Cache;

public interface IRedisCacheService : ICacheService
{
    IDatabase Database { get; }
}
