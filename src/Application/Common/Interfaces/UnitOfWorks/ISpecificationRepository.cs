using System.Linq.Expressions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Specification.Interfaces;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface ISpecificationRepository<T> : IRepository<T>
    where T : class
{
    Task<T?> FindByConditionAsync(
        ISpecification<T> spec,
        CancellationToken cancellationToken = default
    );

    Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class;

    Task<IList<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    );

    Task<IList<TResult>> ListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class;

    Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    );

    Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class;
}
