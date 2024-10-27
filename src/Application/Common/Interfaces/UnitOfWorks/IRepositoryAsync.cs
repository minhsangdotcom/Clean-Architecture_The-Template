using System.Linq.Expressions;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepositoryAsync<T>
    where T : class
{
    Task<IEnumerable<T>> ListAsync();

    Task<T?> FindByIdAsync(object id);

    Task<T?> FindByConditionAsync(Expression<Func<T, bool>> criteria);

    Task<T> AddAsync(T entity);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    Task EditAsync(T entity);

    Task UpdateAsync(T entity);

    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteRangeAsync(IEnumerable<T> entities);

    Task<bool> AnyAsync(Expression<Func<T, bool>>? criteria = null);

    Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null);

    IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null);

    IQueryable<T> Fromsql(string sqlQuery, params object[] parameters);
}
