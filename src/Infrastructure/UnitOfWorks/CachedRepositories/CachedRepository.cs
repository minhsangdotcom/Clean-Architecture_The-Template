using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;
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
        new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromMinutes(5));

    public async Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default) =>
        await repository.ListAsync(cancellationToken);

    public async Task<IEnumerable<TResult>> ListAsync<TResult>(
        CancellationToken cancellationToken = default
    )
        where TResult : class => await repository.ListAsync<TResult>(cancellationToken);

    public async Task<T?> FindByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull => await repository.FindByIdAsync(id, cancellationToken);

    public async Task<T?> FindByConditionAsync(
        Expression<Func<T, bool>> criteria,
        CancellationToken cancellationToken = default
    ) => await repository.FindByConditionAsync(criteria, cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await repository.AddAsync(entity, cancellationToken);

    public async Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    ) => await repository.AddRangeAsync(entities, cancellationToken);

    public async Task EditAsync(T entity) => await repository.EditAsync(entity);

    public async Task UpdateAsync(T entity) => await repository.UpdateAsync(entity);

    public async Task UpdateRangeAsync(IEnumerable<T> entities) =>
        await repository.UpdateRangeAsync(entities);

    public async Task DeleteAsync(T entity) => await repository.DeleteAsync(entity);

    public async Task DeleteRangeAsync(IEnumerable<T> entities) =>
        await repository.DeleteRangeAsync(entities);

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    ) => await repository.AnyAsync(criteria, cancellationToken);

    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    ) => await repository.CountAsync(criteria, cancellationToken);

    public IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null) =>
        repository.ApplyQuery(criteria);

    public IQueryable<T> Fromsql(string sqlQuery, params object[] parameters) =>
        repository.Fromsql(sqlQuery, parameters);
}
