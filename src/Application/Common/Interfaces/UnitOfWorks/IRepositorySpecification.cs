using System.Linq.Expressions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Specs.Interfaces;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepositorySpecification<T>
    where T : class
{
    Task<TResult?> FindByConditionAsync<TResult>(ISpecification<T> spec);

    Task<T?> FindByConditionAsync(ISpecification<T> spec);

    Task<IEnumerable<T>> ListAsync(ISpecification<T> spec, QueryParamRequest queryParam);

    Task<IEnumerable<TResult>> ListWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    );

    Task<PaginationResponse<TResult>> PagedListWithGroupByAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    );

    Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        string? uniqueSort = null
    );

    Task<PaginationResponse<TResult>> CursorPagedListWithGroupByAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null
    );

    IQueryable<T> ApplyQuery(ISpecification<T> spec);
}