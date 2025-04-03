using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Specification.Interfaces;

namespace Application.Common.Interfaces.UnitOfWorks;

/// <summary>
/// To Use static query with Specification for performance
/// </summary>
public interface IStaticPredicateRepository<T>
    where T : class
{
    Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T, TResult> spec,
        CancellationToken cancellationToken = default
    )
        where TResult : class;

    Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T, TResult> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
        where TResult : class;

    Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T, TResult> spec,
        QueryParamRequest queryParam,
        string? uniqueSort = null
    )
        where TResult : class;
}
