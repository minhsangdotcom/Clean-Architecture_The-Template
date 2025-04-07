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
/// <param name="dbContext">IDbcontext</param>
public class SpecificationRepository<T>(IDbContext dbContext) : ISpecificationRepository<T>
    where T : class
{
    public async Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec)
            .Select(mappingResult)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    queryParam.Cursor?.Before,
                    queryParam.Cursor?.After,
                    queryParam.PageSize,
                    queryParam.Sort.GetDefaultSort(),
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    public Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        ApplySpecification(spec).Select(mappingResult).FirstOrDefaultAsync(cancellationToken);

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    ) =>
        ApplySpecification(spec)
            .Select(mappingResult)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize, cancellationToken);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
