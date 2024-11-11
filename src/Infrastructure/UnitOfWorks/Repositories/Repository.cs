using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.UnitOfWorks.Repositories;

public partial class Repository<T>(IDbContext dbContext, IMapper mapper) : IRepository<T>
    where T : class
{
    private readonly IConfigurationProvider _configurationProvider = mapper.ConfigurationProvider;

    public async Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Set<T>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TResult>> ListAsync<TResult>(
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await dbContext
            .Set<T>()
            .ProjectTo<TResult>(_configurationProvider)
            .ToListAsync(cancellationToken);

    public async Task<T?> FindByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull =>
        await dbContext.Set<T>().FindAsync([id], cancellationToken: cancellationToken);

    public async Task<T?> FindByConditionAsync(
        Expression<Func<T, bool>> criteria,
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<T>().Where(criteria).FirstOrDefaultAsync(cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entityEntry = await dbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entityEntry.Entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task EditAsync(T entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(T entity)
    {
        dbContext.Set<T>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        dbContext.Set<T>().UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        dbContext.Set<T>().RemoveRange(entities);
        await Task.CompletedTask;
    }

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<T>().AnyAsync(criteria ?? (x => true), cancellationToken);

    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<T>().CountAsync(criteria ?? (x => true), cancellationToken);

    public IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Where(criteria ?? (x => true));

    public IQueryable<T> Fromsql(string sqlQuery, params object[] parameters) =>
        dbContext.Set<T>().FromSqlRaw(sqlQuery, parameters);
}
