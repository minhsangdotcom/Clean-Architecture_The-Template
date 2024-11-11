using System.Linq.Expressions;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepositoryAsync<T>
    where T : class
{
    Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TResult>> ListAsync<TResult>(CancellationToken cancellationToken = default)
        where TResult : class;

    Task<T?> FindByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull;

    Task<T?> FindByConditionAsync(
        Expression<Func<T, bool>> criteria,
        CancellationToken cancellationToken = default
    );

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    );

    Task EditAsync(T entity);

    Task UpdateAsync(T entity);

    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteRangeAsync(IEnumerable<T> entities);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    );

    Task<int> CountAsync(
        Expression<Func<T, bool>>? criteria = null,
        CancellationToken cancellationToken = default
    );

    IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null);

    IQueryable<T> Fromsql(string sqlQuery, params object[] parameters);
}
