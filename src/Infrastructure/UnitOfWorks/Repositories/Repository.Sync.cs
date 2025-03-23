using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.UnitOfWorks.Repositories;

public partial class Repository<T> : IRepository<T>
    where T : class
{
    public IEnumerable<T> List() => [.. dbContext.Set<T>()];

    public IEnumerable<TResult> List<TResult>()
        where TResult : class => [.. dbContext.Set<T>().ProjectTo<TResult>(_configurationProvider)];

    public T? FindById<TId>(TId id)
        where TId : notnull => dbContext.Set<T>().Find(id);

    public T? FindByCondition(Expression<Func<T, bool>> criteria) =>
        dbContext.Set<T>().Where(criteria).FirstOrDefault();

    public T Add(T entity)
    {
        EntityEntry<T> entityEntry = dbContext.Set<T>().Add(entity);
        return entityEntry.Entity;
    }

    public IEnumerable<T> AddRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().AddRange(entities);
        return entities;
    }

    public void Edit(T entity) => dbContext.Entry(entity).State = EntityState.Modified;

    public void Update(T entity) => dbContext.Set<T>().Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => dbContext.Set<T>().UpdateRange(entities);

    public void Delete(T entity) => dbContext.Set<T>().Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => dbContext.Set<T>().RemoveRange(entities);

    public bool Any(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Any(criteria ?? (x => true));

    public int Count(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Count(criteria ?? (x => true));

    public IEnumerable<T> ApplyQuerySync(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Where(criteria ?? (x => true)).AsEnumerable();
}
