namespace Application.Common.Interfaces.Services.Cache;

public interface ICacheService
{
    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> task, CacheOptions? options = null);
    public T? GetOrSet<T>(string key, Func<T> func, CacheOptions? options = null);
    void Remove(string key);
}
