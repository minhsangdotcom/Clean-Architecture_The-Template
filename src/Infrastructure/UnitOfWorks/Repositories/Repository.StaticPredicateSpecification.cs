using Application.Common.Extensions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Microsoft.EntityFrameworkCore;
using Specification.Evaluators;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.Repositories;

/// <summary>
/// All query like filter, search,sort, DTO Response, pagination define in Specification
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class Repository<T> : IRepository<T>
    where T : class
{
    public async Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T, TResult> spec,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);

    public async Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T, TResult> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize, cancellationToken);

    private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
        where TResult : class =>
        ProjectionSpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
