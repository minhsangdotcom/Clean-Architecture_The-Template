using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Specification.Evaluators;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.Repositories;

/// <summary>
/// do query in Specification
/// </summary>
/// <typeparam name="T"> must be BaseEntity or AggregateRoot</typeparam>
/// <param name="dbContext">IDbContext</param>
public class SpecificationRepository<T>(IDbContext dbContext) : ISpecificationRepository<T>
    where T : class
{
    public Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        ApplySpecification(spec).Select(selector).FirstOrDefaultAsync(cancellationToken);

    public async Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> selector,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec)
            .Select(selector)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    queryParam.Before,
                    queryParam.After,
                    queryParam.PageSize,
                    queryParam.Sort.GetDefaultSort(),
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default
    ) =>
        ApplySpecification(spec)
            .Select(selector)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize, cancellationToken);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
