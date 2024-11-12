using System.Linq.Expressions;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepositorySync<T>
    where T : class
{
    IEnumerable<T> List();

    IEnumerable<TResult> List<TResult>()
        where TResult : class;

    T? FindById<TId>(TId id)
        where TId : notnull;

    T? FindByCondition(Expression<Func<T, bool>> criteria);

    T Add(T entity);

    IEnumerable<T> AddRange(IEnumerable<T> entities);

    void Edit(T entity);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    bool Any(Expression<Func<T, bool>>? criteria = null);

    int Count(Expression<Func<T, bool>>? criteria = null);

    IEnumerable<T> ApplyQuerySync(Expression<Func<T, bool>>? criteria = null);
}
