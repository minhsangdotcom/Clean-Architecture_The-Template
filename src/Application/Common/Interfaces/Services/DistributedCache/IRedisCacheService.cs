using Application.Common.Interfaces.Registers;
using StackExchange.Redis;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IRedisCacheService : ISingleton
{
    IDatabase Database { get; }
}