namespace Application.Common.Interfaces.Services.Cache;

public interface IDistributedCacheService : ICacheService
{
    Task RemoveAsync(string key);
}
