using System.Linq.Expressions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Specs.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public partial class CachedRepository<T> : IRepository<T>
    where T : class
{
    public IQueryable<T> ApplyQuery(ISpecification<T> spec) => repository.ApplyQuery(spec);

    public Task<TResult?> FindByConditionAsync<TResult>(ISpecification<T> spec)
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            logger.Information("checking cache for {key}", key);
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.FindByConditionAsync<TResult>(spec);
                }
            );
        }
        return repository.FindByConditionAsync<TResult>(spec);
    }

    public Task<T?> FindByConditionAsync(ISpecification<T> spec)
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            logger.Information("checking cache for {key}", key);
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.FindByConditionAsync(spec);
                }
            );
        }
        return repository.FindByConditionAsync(spec);
    }

    public Task<IEnumerable<T>> ListAsync(ISpecification<T> spec, QueryParamRequest queryParam)
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListAsync)}";
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.ListAsync(spec, queryParam);
                }
            )!;
        }
        return repository.ListAsync(spec, queryParam);
    }

    //? i will come up with the best idea for groupby
    public Task<IEnumerable<TResult>> ListWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListWithGroupbyAsync)}";
            logger.Information("checking cache for {key}", key);
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.ListWithGroupbyAsync<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression
                    );
                }
            )!;
        }
        return repository.ListWithGroupbyAsync<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression
        );
    }

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListAsync)}";
            logger.Information("checking cache for {key}", key);
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.PagedListAsync<TResult>(spec, queryParam);
                }
            )!;
        }
        return repository.PagedListAsync<TResult>(spec, queryParam);
    }

    public PaginationResponse<TResult> PagedList<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedList)}";
            logger.Information("checking cache for {key}", key);
            return cache.GetOrCreate(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.PagedList<TResult>(spec, queryParam);
                }
            )!;
        }
        return repository.PagedList<TResult>(spec, queryParam);
    }

    //? i will come up with the best idea for groupby
    public Task<PaginationResponse<TResult>> PagedListWithGroupByAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListWithGroupByAsync)}";
            return cache.GetOrCreate(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.PagedListWithGroupByAsync<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression
                    );
                }
            )!;
        }
        return repository.PagedListWithGroupByAsync<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression
        );
    }

    public PaginationResponse<TResult> PagedListWithGroupBy<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListWithGroupBy)}";
            return cache.GetOrCreate(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.PagedListWithGroupBy<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression
                    );
                }
            )!;
        }
        return repository.PagedListWithGroupBy<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression
        );
    }

    public Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        string? uniqueSort = null!
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(CursorPagedListAsync)}";
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.CursorPagedListAsync<TResult>(spec, queryParam, uniqueSort);
                }
            )!;
        }
        return repository.CursorPagedListAsync<TResult>(spec, queryParam, uniqueSort);
    }

    //? i will come up with the best idea for groupby
    public Task<PaginationResponse<TResult>> CursorPagedListWithGroupByAsync<
        TGroupProperty,
        TResult
    >(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        string? uniqueSort = null
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(CursorPagedListWithGroupByAsync)}";
            return cache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", key);
                    return repository.CursorPagedListWithGroupByAsync<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression,
                        uniqueSort
                    );
                }
            )!;
        }
        return repository.CursorPagedListWithGroupByAsync<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression,
            uniqueSort
        );
    }
}
