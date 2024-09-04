using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public partial class Repository<T>(IDbContext dbContext, IMapper mapper) :
    IRepository<T>
    where T : BaseEntity
{
    private readonly IConfigurationProvider _configurationProvider = mapper.ConfigurationProvider;

    public async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await dbContext.Set<T>().AddRangeAsync(entities);

        return entities;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await dbContext.Set<T>().AnyAsync(expression);
    }

    public async Task<T?> GetAsync(object id)
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> ListAsync()
    {
        List<T> list = await dbContext.Set<T>().ToListAsync();

        return list;
    }

    public async Task ModifyAsync(T entity)
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

    public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await dbContext.Set<T>().CountAsync(expression);
    }

    public IQueryable<T> ApplyQuery(Expression<Func<T, bool>> criteria = null!)
    {
        Expression<Func<T, bool>> expression = criteria ?? (x => true);

        return dbContext.Set<T>().Where(expression);
    }

    public IQueryable<T> Fromsql(string sqlQuery, params object[] parameters) =>
        dbContext.Set<T>().FromSqlRaw(sqlQuery, parameters);
}