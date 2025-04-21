using StackExchange.Redis;

namespace Application.Common.Interfaces.Services.Cache;

public interface IDistributedCacheService : ICacheService
{
    IDatabase Database { get; }
}
