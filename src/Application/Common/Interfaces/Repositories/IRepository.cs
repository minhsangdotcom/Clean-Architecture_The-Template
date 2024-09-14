using System.Linq.Expressions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Common;
using Domain.Specs.Interfaces;

namespace Application.Common.Interfaces.Repositories;

public interface IRepository<T>
    : IRepositoryAsync<T>,
        IRepositorySync<T>,
        IRepositorySpecification<T>
    where T : BaseEntity { }

public interface IRepositoryAsync<T>
    where T : BaseEntity
{
    Task<IEnumerable<T>> ListAsync();

    Task<T?> GetAsync(object id);

    Task<T> AddAsync(T entity);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    Task ModifyAsync(T entity);

    Task UpdateAsync(T entity);

    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteRangeAsync(IEnumerable<T> entities);

    IQueryable<T> ApplyQuery(Expression<Func<T, bool>> criteria = null!);

    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

    Task<int> CountAsync(Expression<Func<T, bool>> expression);

    IQueryable<T> Fromsql(string sqlQuery, params object[] parameters);
}

public interface IRepositorySync<T>
    where T : BaseEntity
{
    IEnumerable<T> List();

    T? Get(object id);

    T Add(T entity);

    IEnumerable<T> AddRange(IEnumerable<T> entities);

    void Modify(T entity);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    bool Any(Expression<Func<T, bool>> expression);

    int Count(Expression<Func<T, bool>> expression);
}

public interface IRepositorySpecification<T>
    where T : BaseEntity
{
    IQueryable<T> ApplyQuery(ISpecification<T> spec);

    Task<TResult?> GetByConditionSpecificationAsync<TResult>(ISpecification<T> spec);

    Task<T?> GetByConditionSpecificationAsync(ISpecification<T> spec);

    Task<IEnumerable<T>> ListWithSpecificationAsync(ISpecification<T> spec, QueryRequest request);

    Task<IEnumerable<TResult>> ListSpecificationWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> PaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryRequest request
    );

    Task<PaginationResponse<TResult>> PaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryRequest request,
        string? uniqueSort = null!
    );

    Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null!
    );
}
