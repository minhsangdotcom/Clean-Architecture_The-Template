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

    public async Task<TResult?> GetByConditionSpecificationAsync<TResult>(ISpecification<T> spec) =>
        await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .FirstOrDefaultAsync();

    public async Task<T?> GetByConditionSpecificationAsync(ISpecification<T> spec) =>
        await ApplySpecification(spec).FirstOrDefaultAsync();

    public async Task<PaginationResponse<TResult>> PaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest request
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";
        Search? search = request.Search;

        return await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(request.Page, request.PageSize);
    }

    public async Task<PaginationResponse<TResult>> PaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = request.Search;

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(request.Page, request.PageSize);
    }

    public async Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest request,
        string? uniqueSort = null!
    ) =>
        await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Search?.Keyword, request.Search?.Targets)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    request.Cursor?.Before,
                    request.Cursor?.After,
                    request.PageSize,
                    request.Sort,
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    public async Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null
    ) =>
        await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Search?.Keyword, request.Search?.Targets)
            .ToCursorPagedListAsync(
                new CursorPaginationRequest(
                    request.Cursor?.Before,
                    request.Cursor?.After,
                    request.PageSize,
                    request.Sort,
                    uniqueSort ?? nameof(BaseEntity.Id)
                )
            );

    public async Task<IEnumerable<T>> ListWithSpecificationAsync(
        ISpecification<T> spec,
        QueryParamRequest request
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = request.Search;

        return await ApplySpecification(spec)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToListAsync();
    }

    public async Task<IEnumerable<TResult>> ListSpecificationWithGroupbyAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest request,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";

        Search? search = request.Search;

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToListAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
}
