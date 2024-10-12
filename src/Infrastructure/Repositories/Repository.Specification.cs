using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using AutoMapper.QueryableExtensions;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.QueryExtensions;
using Domain.Common;
using Domain.Specs;
using Domain.Specs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public partial class Repository<T> : IRepository<T>
    where T : class
{
    public IQueryable<T> ApplyQuery(ISpecification<T> spec) => ApplySpecification(spec);

    public async Task<TResult?> FindByConditionAsync<TResult>(ISpecification<T> spec) =>
        await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .FirstOrDefaultAsync();

    public async Task<T?> FindByConditionAsync(ISpecification<T> spec) =>
        await ApplySpecification(spec).FirstOrDefaultAsync();

    public async Task<IEnumerable<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(queryParam.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : queryParam.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .Filter(queryParam.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToListAsync();
    }

    //? i will come up with the best idea for groupby
    public async Task<IEnumerable<TResult>> ListWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(queryParam.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : queryParam.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToListAsync();
    }

    public async Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(queryParam.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : queryParam.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";
        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Filter(queryParam.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize);
    }

    //? i will come up with the best idea for groupby
    public async Task<PaginationResponse<TResult>> PagedListWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(queryParam.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : queryParam.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = queryParam.Search;

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(queryParam.Page, queryParam.PageSize);
    }

    public async Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        string? uniqueSort = null!
    ) =>
        await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Filter(queryParam.DynamicFilter)
            .Search(queryParam.Search?.Keyword, queryParam.Search?.Targets)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    queryParam.Cursor?.Before,
                    queryParam.Cursor?.After,
                    queryParam.PageSize,
                    queryParam.Sort,
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    //? i will come up with the best idea for groupby
    public async Task<PaginationResponse<TResult>> CursorPagedListWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null
    ) =>
        await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(queryParam.Search?.Keyword, queryParam.Search?.Targets)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    queryParam.Cursor?.Before,
                    queryParam.Cursor?.After,
                    queryParam.PageSize,
                    queryParam.Sort,
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
