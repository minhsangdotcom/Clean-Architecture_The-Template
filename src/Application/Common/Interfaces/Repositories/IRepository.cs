using System.Linq.Expressions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Specs.Interfaces;

namespace Application.Common.Interfaces.Repositories;

public interface IRepository<T>
    : IRepositoryAsync<T>,
        IRepositorySync<T>,
        IRepositorySpecification<T>
    where T : class { }

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

    Task<bool> AnyAsync(Expression<Func<T, bool>> criteria);

    Task<int> CountAsync(Expression<Func<T, bool>> criteria);

    IQueryable<T> ApplyQuery(Expression<Func<T, bool>>? criteria = null);
    
    IQueryable<T> Fromsql(string sqlQuery, params object[] parameters);
}

public interface IRepositorySync<T>
    where T : class
{
    IEnumerable<T> List();

    T? FindById(object id);

    T? FindByCondition(Expression<Func<T, bool>> criteria);

    T Add(T entity);

    IEnumerable<T> AddRange(IEnumerable<T> entities);

    void Edit(T entity);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    bool Any(Expression<Func<T, bool>> criteria);

    int Count(Expression<Func<T, bool>> criteria);

    IEnumerable<T> ApplyQuerySync(Expression<Func<T, bool>>? criteria = null);
}

public interface IRepositorySpecification<T>
    where T : class
{
    IQueryable<T> ApplyQuery(ISpecification<T> spec);

    Task<TResult?> GetByConditionSpecificationAsync<TResult>(ISpecification<T> spec);

    Task<T?> GetByConditionSpecificationAsync(ISpecification<T> spec);

    Task<IEnumerable<T>> ListWithSpecificationAsync(
        ISpecification<T> spec,
        QueryParamRequest request
    );

    Task<IEnumerable<TResult>> ListSpecificationWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> PaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest request
    );

    Task<PaginationResponse<TResult>> PaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest request,
        string? uniqueSort = null!
    );

    Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null!
    );
}
