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
/// specification just do where include and extensions will do the rest such as :filter,search,sort,pagination
/// </summary>
/// <typeparam name="T">must be BaseEntity or AggregateRoot</typeparam>
/// <param name="dbContext">must be IDbcontext</param>
public class SpecificationRepository<T>(IDbContext dbContext) : ISpecificationRepository<T>
    where T : class
{
    public async Task<T?> FindByConditionAsync(
        ISpecification<T> spec,
        CancellationToken cancellationToken = default
    ) => await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);

    public async Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec).Select(mappingResult).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
    {
        string uniqueSort = queryParam.Sort.GetSort();
        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .Filter(queryParam.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TResult>> ListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        string uniqueSort = queryParam.Sort.GetSort();
        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .Filter(queryParam.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .Select(mappingResult)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
    {
        string uniqueSort = queryParam.Sort.GetSort();
        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .Filter(queryParam.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .Select(mappingResult)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize, cancellationToken);
    }

    public async Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class =>
        await ApplySpecification(spec)
            .Filter(queryParam.DynamicFilter)
            .Search(queryParam.Search?.Keyword, queryParam.Search?.Targets)
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

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
