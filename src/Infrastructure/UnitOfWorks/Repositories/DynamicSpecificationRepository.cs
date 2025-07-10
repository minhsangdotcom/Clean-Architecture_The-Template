using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Extensions.QueryExtensions;
using Specification.Evaluators;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.Repositories;

/// <summary>
/// combine specification pattern and dynamic queries
/// </summary>
/// <typeparam name="T">must be BaseEntity or AggregateRoot</typeparam>
/// <param name="dbContext">must be IDbContext</param>
public class DynamicSpecificationRepository<T>(IDbContext dbContext)
    : IDynamicSpecificationRepository<T>
    where T : class
{
    public async Task<T?> FindByConditionAsync(
        ISpecification<T> spec,
        CancellationToken cancellationToken = default
    ) => await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);

    public async Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec).Select(selector).FirstOrDefaultAsync(cancellationToken);

    public async Task<IList<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        int deep = 1,
        CancellationToken cancellationToken = default
    )
    {
        string uniqueSort = queryParam.Sort.GetSort();

        return await ApplySpecification(spec)
            .Filter(queryParam.Filter)
            .Search(queryParam.Keyword, queryParam.Targets, deep)
            .Sort(uniqueSort)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<TResult>> ListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> selector,
        int deep = 1,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        string uniqueSort = queryParam.Sort.GetSort();

        return await ApplySpecification(spec)
            .Filter(queryParam.Filter)
            .Search(queryParam.Keyword, queryParam.Targets, deep)
            .Sort(uniqueSort)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> selector,
        int deep = 1,
        CancellationToken cancellationToken = default
    )
    {
        string uniqueSort = queryParam.Sort.GetSort();

        return await ApplySpecification(spec)
            .Filter(queryParam.Filter)
            .Search(queryParam.Keyword, queryParam.Targets, deep)
            .Sort(uniqueSort)
            .Select(selector)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize, cancellationToken);
    }

    public async Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> selector,
        int deep = 1,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec)
            .Filter(queryParam.Filter)
            .Search(queryParam.Keyword, queryParam.Targets, deep)
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

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
