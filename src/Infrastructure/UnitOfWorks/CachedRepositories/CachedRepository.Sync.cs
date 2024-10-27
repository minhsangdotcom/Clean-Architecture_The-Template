using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public partial class CachedRepository<T> : IRepository<T>
    where T : class
{
    public IEnumerable<T> List() => repository.List();

    public T? FindById(object id) => repository.FindById(id);

    public T? FindByCondition(Expression<Func<T, bool>> criteria) =>
        repository.FindByCondition(criteria);

    public T Add(T entity) => repository.Add(entity);

    public IEnumerable<T> AddRange(IEnumerable<T> entities) => repository.AddRange(entities);

    public void Edit(T entity) => repository.Edit(entity);

    public void Update(T entity) => repository.Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => repository.UpdateRange(entities);

    public void Delete(T entity) => repository.Delete(entity);

    public void DeleteRange(IEnumerable<T> entities) => repository.DeleteRange(entities);

    public bool Any(Expression<Func<T, bool>>? criteria = null) => repository.Any(criteria);

    public int Count(Expression<Func<T, bool>>? criteria = null) => repository.Count(criteria);

    public IEnumerable<T> ApplyQuerySync(Expression<Func<T, bool>>? criteria = null) =>
        repository.ApplyQuerySync(criteria);
}
