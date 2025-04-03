using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Extensions.QueryExtensions;
using SharedKernel.Models;
using Specification.Evaluators;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.Repositories;

public partial class Repository<T> : IRepository<T>
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
        string uniqueSort = GetSort(queryParam.Sort);
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
        string uniqueSort = GetSort(queryParam.Sort);
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
        string uniqueSort = GetSort(queryParam.Sort);
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
                    GetDefaultSort(queryParam.Sort),
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    public static string GetSort(string? sort)
    {
        string defaultSort = GetDefaultSort(sort);
        return $"{defaultSort},{nameof(BaseEntity.Id)}";
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);

    private static string GetDefaultSort(string? sort) =>
        string.IsNullOrWhiteSpace(sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : sort.Trim();
}
