namespace Application.Common.Interfaces.Services.Cache;

public interface ICacheService
{
    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> task, TimeSpan? expiry = null);
    public T? GetOrSet<T>(string key, Func<T> func, TimeSpan? expiry = null);
}
