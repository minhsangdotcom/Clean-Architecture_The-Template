using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces.UnitOfWorks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SharedKernel.Common.Specs.Interfaces;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public partial class CachedRepository<T> : IRepository<T>
    where T : class
{
    public IQueryable<T> ApplyQuery(ISpecification<T> spec) => repository.ApplyQuery(spec);

    public Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            string hashingKey = HashKey(key);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.FindByConditionAsync<TResult>(spec, cancellationToken);
                }
            );
        }
        return repository.FindByConditionAsync<TResult>(spec, cancellationToken);
    }

    public Task<T?> FindByConditionAsync(
        ISpecification<T> spec,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            string hashingKey = HashKey(key);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.FindByConditionAsync(spec, cancellationToken);
                }
            );
        }
        return repository.FindByConditionAsync(spec, cancellationToken);
    }

    public Task<IEnumerable<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListAsync)}";
            string hashingKey = HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.ListAsync(spec, queryParam, cancellationToken);
                }
            )!;
        }
        return repository.ListAsync(spec, queryParam, cancellationToken);
    }

    //? i will come up with the best idea for groupby
    public Task<IEnumerable<TResult>> ListWithGroupbyAsync<TGroupProperty, TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TGroupProperty>> groupByExpression,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListWithGroupbyAsync)}";
            string hashingKey = HashKey(key, queryParam, groupByExpression.ToString());
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.ListWithGroupbyAsync<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression,
                        cancellationToken
                    );
                }
            )!;
        }
        return repository.ListWithGroupbyAsync<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression,
            cancellationToken
        );
    }

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListAsync)}";
            string hashingKey = HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.PagedListAsync<TResult>(spec, queryParam, cancellationToken);
                }
            )!;
        }
        return repository.PagedListAsync<TResult>(spec, queryParam, cancellationToken);
    }

    public PaginationResponse<TResult> PagedList<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedList)}";
            string hashingKey = HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreate(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
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
        Expression<Func<T, TGroupProperty>> groupByExpression,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListWithGroupByAsync)}";
            string hashingKey = HashKey(key, queryParam, groupByExpression.ToString()!);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreate(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
                    return repository.PagedListWithGroupByAsync<TGroupProperty, TResult>(
                        spec,
                        queryParam,
                        groupByExpression,
                        cancellationToken
                    );
                }
            )!;
        }
        return repository.PagedListWithGroupByAsync<TGroupProperty, TResult>(
            spec,
            queryParam,
            groupByExpression,
            cancellationToken
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
            string hashingKey = HashKey(key, queryParam, groupByExpression.ToString());
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreate(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
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
            string hashingKey = HashKey(key, queryParam, uniqueSort);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
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
            string hashingKey = HashKey(key, queryParam, uniqueSort);
            logger.Information("checking cache for {key}", hashingKey);
            return cache.GetOrCreateAsync(
                hashingKey,
                entry =>
                {
                    entry.SetOptions(cacheOptions);
                    logger.Warning("fetching source for {key}", hashingKey);
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

    private static string HashKey(params object?[] parameters)
    {
        StringBuilder text = new();
        foreach (object? param in parameters)
        {
            if (param is null)
            {
                continue;
            }

            if (param is string)
            {
                AppendParameter(text, param.ToString()!);
                continue;
            }

            var result = JsonConvert.SerializeObject(param);
            AppendParameter(text, result);
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString()));
        return Convert.ToHexString(bytes);
    }

    private static StringBuilder AppendParameter(StringBuilder text, string param)
    {
        if (!string.IsNullOrWhiteSpace(text.ToString()))
        {
            text.Append('_');
        }
        text.Append(param);

        return text;
    }
}
