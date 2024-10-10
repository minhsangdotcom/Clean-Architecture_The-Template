using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Infrastructure.Repositories;

public partial class Repository<T> : IRepository<T>
    where T : class
{
    public IEnumerable<T> List() => [.. dbContext.Set<T>()];

    public T? FindById(object id) => dbContext.Set<T>().Find(id);

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

    public bool Any(Expression<Func<T, bool>> criteria) => dbContext.Set<T>().Any(criteria);

    public int Count(Expression<Func<T, bool>> criteria) => dbContext.Set<T>().Count(criteria);

    public IEnumerable<T> ApplyQuerySync(Expression<Func<T, bool>>? criteria = null) =>
        dbContext.Set<T>().Where(criteria ?? (x => true)).AsEnumerable();
}
