using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Serilog;
using Specification.Interfaces;

namespace Infrastructure.UnitOfWorks.CachedRepositories;

public class CachedStaticPredicateSpecificationRepository<T>(
    IStaticPredicateSpecificationRepository<T> repository,
    ILogger logger,
    IMemoryCacheService memoryCacheService
) : IStaticPredicateSpecificationRepository<T>
    where T : class
{
    public Task<PaginationResponse<TResult>> CursorPagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        System.Linq.Expressions.Expression<Func<T, TResult>> mappingResult,
        string? uniqueSort = null,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(CursorPagedListAsync)}";
            string hashingKey = RepositoryExtention.HashKey(key, queryParam, uniqueSort);
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

    public Task<TResult?> FindByConditionAsync<TResult>(
        ISpecification<T> spec,
        System.Linq.Expressions.Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
        where TResult : class
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(FindByConditionAsync)}";
            string hashingKey = RepositoryExtention.HashKey(key);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.FindByConditionAsync(spec, mappingResult, cancellationToken)
            );
        }
        return repository.FindByConditionAsync(spec, mappingResult, cancellationToken);
    }

    public Task<PaginationResponse<TResult>> PagedListAsync<TResult>(
        ISpecification<T> spec,
        QueryParamRequest queryParam,
        System.Linq.Expressions.Expression<Func<T, TResult>> mappingResult,
        CancellationToken cancellationToken = default
    )
    {
        if (spec.CacheEnabled)
        {
            string key = $"{spec.CacheKey}-{nameof(PagedListAsync)}";
            string hashingKey = RepositoryExtention.HashKey(key, queryParam);
            logger.Information("checking cache for {key}", hashingKey);
            return memoryCacheService.GetOrSetAsync(
                hashingKey,
                () => repository.PagedListAsync(spec, queryParam, mappingResult, cancellationToken)
            )!;
        }
        return repository.PagedListAsync(spec, queryParam, mappingResult, cancellationToken);
    }
}
