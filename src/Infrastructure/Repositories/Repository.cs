using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public partial class Repository<T>(IDbContext dbContext, IMapper mapper) : IRepository<T>
    where T : class
{
    private readonly IConfigurationProvider _configurationProvider = mapper.ConfigurationProvider;

    public async Task<IEnumerable<T>> ListAsync() => await dbContext.Set<T>().ToListAsync();

    public async Task<T?> FindByIdAsync(object id) => await dbContext.Set<T>().FindAsync(id);

    public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> criteria) =>
        await dbContext.Set<T>().Where(criteria).FirstOrDefaultAsync();

    public async Task<T> AddAsync(T entity)
    {
        EntityEntry<T> entityEntry = await dbContext.Set<T>().AddAsync(entity);
        return entityEntry.Entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await dbContext.Set<T>().AddRangeAsync(entities);
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

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria) =>
        await dbContext.Set<T>().AnyAsync(criteria);

    public async Task<int> CountAsync(Expression<Func<T, bool>> criteria) =>
        await dbContext.Set<T>().CountAsync(criteria);

    public IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Where(criteria ?? (x => true));

    public IQueryable<T> Fromsql(string sqlQuery, params object[] parameters) =>
        dbContext.Set<T>().FromSqlRaw(sqlQuery, parameters);
}
