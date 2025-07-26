namespace Application.Common.Interfaces.Services.Cache;

public interface IMemoryCacheService : ICacheService
{
    bool HasKey(string key);
}
