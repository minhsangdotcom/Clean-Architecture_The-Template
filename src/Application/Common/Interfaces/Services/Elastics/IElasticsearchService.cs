using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Application.Common.Interfaces.Services.Elastics;

public interface IElasticsearchService<T>
    where T : class
{
    Task<IEnumerable<T>> ListAsync();

    Task<T?> GetAsync(object id);

    Task<T> AddAsync(T entity);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);

    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteRangeAsync(IEnumerable<T> entities);

    Task UpdateByQueryAsync(T entity, string query, Dictionary<string, object> parameters);

    Task DeleteByQueryAsync(Action<QueryDescriptor<T>> querySelector);

    Task<bool> AnyAsync(Action<BoolQueryDescriptor<T>> selector);

    Task<long> CountAsync(CountRequestDescriptor<T> selector);

    Task<PaginationResponse<TResult>> PaginatedListAsync<TResult>(
        Action<QueryDescriptor<T>> filter,
        QueryRequest request
    );
}
