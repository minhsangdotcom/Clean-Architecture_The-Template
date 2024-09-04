using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using AutoMapper.QueryableExtensions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.QueryExtensions;
using Domain.Common;
using Domain.Specs;
using Domain.Specs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public partial class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    public IQueryable<T> ApplyQuery(ISpecification<T> spec)
    {
        return ApplySpecification(spec);
    }

    public async Task<TResult?> GetByConditionSpecificationAsync<TResult>(ISpecification<T> spec) =>
        await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .FirstOrDefaultAsync();

    public async Task<T?> GetByConditionSpecificationAsync(ISpecification<T> spec) =>
        await ApplySpecification(spec).FirstOrDefaultAsync();

    public async Task<PaginationResponse<TResult>> PaginatedListSpecificationAsync<TResult>(ISpecification<T> spec, QueryRequest request)
    {
        string sortRequest = string.IsNullOrWhiteSpace(request.Order!) ? $"{nameof(BaseEntity.CreatedAt)} desc" : request.Order;

        string orderQuery = $"{sortRequest},{nameof(BaseEntity.Id)}";

        return await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Keyword, request.SearchTarget)
            .Sort(orderQuery)
            .PaginateAsync(request.CurrentPage, request.Size);
    }

    public async Task<PaginationResponse<TResult>> PaginatedListSpecificationWithGroupByAsync<TGroupProperty, TResult>(ISpecification<T> spec, QueryRequest request, Expression<Func<T, TGroupProperty>> groupByExpression)
    {
        string sortRequest = string.IsNullOrWhiteSpace(request.Order) ? $"{nameof(BaseEntity.CreatedAt)} desc" : request.Order;

        string orderQuery = $"{sortRequest},{nameof(BaseEntity.Id)}";

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Keyword, request.SearchTarget)
            .Sort(orderQuery)
            .PaginateAsync(request.CurrentPage, request.Size);
    }

    public async Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationAsync<TResult>(ISpecification<T> spec, QueryRequest request, string? uniqueSort = null!)
    {
        return await ApplySpecification(spec)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Keyword, request.SearchTarget)
            //.PointerPaginateAsync(request.Cursor, request.Size, request.Order!, uniqueSort ?? nameof(BaseEntity.Id));
            .PointerPaginateAsync(new CursorPaginationRequest(request.Before, request.After, request.Size, request.Order, uniqueSort ?? nameof(BaseEntity.Id)));
    }

    public async Task<PaginationResponse<TResult>> CursorPaginatedListSpecificationWithGroupByAsync<TGroupProperty, TResult>(ISpecification<T> spec, QueryRequest request, Expression<Func<T, TGroupProperty>> groupByExpression, string? uniqueSort = null)
    {
        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Keyword, request.SearchTarget)
            //.PointerPaginateAsync(request.Cursor, request.Size, request.Order!, uniqueSort ?? nameof(BaseEntity.Id));
            .PointerPaginateAsync(new CursorPaginationRequest(request.Before, request.After, request.Size, request.Order, uniqueSort ?? nameof(BaseEntity.Id)));
    }

    public async Task<IEnumerable<T>> ListWithSpecificationAsync(ISpecification<T> spec, QueryRequest request)
    {
        string sortRequest = string.IsNullOrWhiteSpace(request.Order!) ? $"{nameof(BaseEntity.CreatedAt)} desc" : request.Order;

        string orderQuery = $"{sortRequest},{nameof(BaseEntity.Id)}";

        return await ApplySpecification(spec)
            .Search(request.Keyword, request.SearchTarget)
            .Sort(orderQuery)
            .ToListAsync();
    }

    public async Task<IEnumerable<TResult>> ListSpecificationWithGroupbyAsync<TGroupProperty, TResult>(ISpecification<T> spec, QueryRequest request, Expression<Func<T, TGroupProperty>> groupByExpression)
    {
        string sortRequest = string.IsNullOrWhiteSpace(request.Order!) ? $"{nameof(BaseEntity.CreatedAt)} desc" : request.Order;

        string orderQuery = $"{sortRequest},{nameof(BaseEntity.Id)}";

        return await ApplySpecification(spec)
            .GroupBy(groupByExpression)
            .ProjectTo<TResult>(_configurationProvider)
            .Search(request.Keyword, request.SearchTarget)
            .Sort(orderQuery)
            .ToListAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
    }
}