using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public partial class CachedRepository<T>(
    IRepository<T> repository,
    IMemoryCache cache,
    ILogger logger
) : IRepository<T>
    where T : class
{
    private readonly MemoryCacheEntryOptions cacheOptions =
        new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            relative: TimeSpan.FromMinutes(5)
        );

    public async Task<IEnumerable<T>> ListAsync() => await repository.ListAsync();

    public async Task<T?> FindByIdAsync(object id) => await repository.FindByIdAsync(id);

    public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> criteria) =>
        await repository.FindByConditionAsync(criteria);

    public async Task<T> AddAsync(T entity) => await repository.AddAsync(entity);

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities) =>
        await repository.AddRangeAsync(entities);

    public async Task EditAsync(T entity) => await repository.EditAsync(entity);

    public async Task UpdateAsync(T entity) => await repository.UpdateAsync(entity);

    public async Task UpdateRangeAsync(IEnumerable<T> entities) =>
        await repository.UpdateRangeAsync(entities);

    public async Task DeleteAsync(T entity) => await repository.DeleteAsync(entity);

    public async Task DeleteRangeAsync(IEnumerable<T> entities) =>
        await repository.DeleteRangeAsync(entities);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>>? criteria = null) =>
        await repository.AnyAsync(criteria);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null) =>
        await repository.CountAsync(criteria);

    public IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null) =>
        repository.ApplyQuery(criteria);

    public IQueryable<T> Fromsql(string sqlQuery, params object[] parameters) =>
        repository.Fromsql(sqlQuery, parameters);
}
