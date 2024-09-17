using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public partial class Repository<T> : IRepository<T>
    where T : class
{
    public void Modify(T entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().RemoveRange(entities);
    }

    public IEnumerable<T> List()
    {
       return dbContext.Set<T>().ToList();
    }

    public T? Get(object id)
    {
        return dbContext.Set<T>().Find(id);
    }

    public T Add(T entity)
    {
        dbContext.Set<T>().Add(entity);
        return entity;
    }

    public IEnumerable<T> AddRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().AddRange(entities);
        return entities;
    }

    public bool Any(Expression<Func<T, bool>> expression)
    {
        return dbContext.Set<T>().Any(expression);
    }

    public int Count(Expression<Func<T, bool>> expression)
    {
        return dbContext.Set<T>().Count(expression);
    }
}