using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Newtonsoft.Json;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public partial class CachedRepository<T> : IRepository<T>
    where T : class
{
    public Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            string hashingKey = HashKey(key);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.FindByConditionAsync(spec, mappingResult, cancellationToken)
            );
        }
        return repository.FindByConditionAsync(spec, mappingResult, cancellationToken);
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
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.FindByConditionAsync(spec, cancellationToken)
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
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.ListAsync(spec, queryParam, cancellationToken)
            )!;
        }
        return repository.ListAsync(spec, queryParam, cancellationToken);
    }

    public Task<IEnumerable<TResult>> ListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListAsync)}";
            string hashingKey = HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.ListAsync(spec, queryParam, mappingResult, cancellationToken)
            )!;
        }
        return repository.ListAsync(spec, queryParam, mappingResult, cancellationToken);
    }

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListAsync)}";
            string hashingKey = HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.PagedListAsync(spec, queryParam, mappingResult, cancellationToken)
            )!;
        }
        return repository.PagedListAsync(spec, queryParam, mappingResult, cancellationToken);
    }

    public Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        Expression<Func<T, TResult>> mappingResult,
        string? uniqueSort = null!,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(CursorPagedListAsync)}";
            string hashingKey = HashKey(key, queryParam, uniqueSort);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () =>
                    repository.CursorPagedListAsync(
                        spec,
                        queryParam,
                        mappingResult,
                        uniqueSort,
                        cancellationToken
                    )
            )!;
        }
        return repository.CursorPagedListAsync(
            spec,
            queryParam,
            mappingResult,
            uniqueSort,
            cancellationToken
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
