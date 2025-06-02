using System.Linq.Expressions;
using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Serilog;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public class CachedDynamicSpecRepository<T>(
    IDynamicSpecificationRepository<T> repository,
    ILogger logger,
    IMemoryCacheService memoryCacheService
) : IDynamicSpecificationRepository<T>
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
            string hashingKey = RepositoryExtension.HashKey(key);
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
            string hashingKey = RepositoryExtension.HashKey(key);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.FindByConditionAsync(spec, cancellationToken)
            );
        }
        return repository.FindByConditionAsync(spec, cancellationToken);
    }

    public Task<IList<T>> ListAsync(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(ListAsync)}";
            string hashingKey = RepositoryExtension.HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.ListAsync(spec, queryParam, cancellationToken)
            )!;
        }
        return repository.ListAsync(spec, queryParam, cancellationToken);
    }

    public Task<IList<TResult>> ListAsync<TResult>(
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
            string hashingKey = RepositoryExtension.HashKey(key, queryParam);
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
            string hashingKey = RepositoryExtension.HashKey(key, queryParam);
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
            string hashingKey = RepositoryExtension.HashKey(key, queryParam, uniqueSort);
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
}
