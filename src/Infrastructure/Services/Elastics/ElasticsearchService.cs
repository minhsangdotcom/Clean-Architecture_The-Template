using Application.Common.Interfaces.Services.Elastics;
using AutoMapper;
using Domain.Common.ElasticConfigurations;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Fluent;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Infrastructure.Services.Elastics;

public class ElasticsearchService<T>(ElasticsearchClient elasticClient) : IElasticsearchService<T>
    where T : class
{
    private readonly string indexName = ElsIndexExtension.GetName<T>();

    public async Task<T> AddAsync(T entity)
    {
        await elasticClient.IndexAsync(entity, i => i.Refresh(Refresh.WaitFor).Index(indexName));

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await elasticClient.BulkAsync(x =>
            x.Index(indexName).CreateMany(entities).Refresh(Refresh.WaitFor)
        );

        return entities;
    }

    public async Task<bool> AnyAsync(Action<BoolQueryDescriptor<T>> selector)
    {
        SearchResponse<T> searchResponse = await elasticClient.SearchAsync<T>(s =>
            s.Query(q => q.Bool(selector)).Index(indexName)
        );

        return searchResponse.Documents.Count != 0;
    }

    public async Task<long> CountAsync(CountRequestDescriptor<T> selector)
    {
        CountResponse countResponse = await elasticClient.CountAsync<T>(x =>
            selector.Indices(indexName)
        );
        return countResponse.Count;
    }

    public async Task DeleteAsync(T entity)
    {
        await elasticClient.DeleteAsync<T>(
            entity,
            i => i.Index(indexName).Refresh(Refresh.WaitFor)
        );
    }

    public async Task DeleteByQueryAsync(Action<QueryDescriptor<T>> querySelector)
    {
        await elasticClient.DeleteByQueryAsync<T>(x => x.Indices(indexName).Query(querySelector));
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        await elasticClient.BulkAsync(x =>
            x.Index(indexName).DeleteMany(entities).Refresh(Refresh.WaitFor)
        );
    }

    public async Task<T?> GetAsync(object id)
    {
        GetResponse<T> getResponse = await elasticClient.GetAsync<T>(
            id.ToString()!,
            idx => idx.Index(indexName)
        );

        return getResponse.Source;
    }

    public async Task<IEnumerable<T>> ListAsync()
    {
        SearchResponse<T> searchResponse = await elasticClient.SearchAsync<T>(s =>
            s.Index(indexName)
        );

        return searchResponse.Documents;
    }

    public async Task ModifyAsync(T entity)
    {
        await UpdateAsync(entity);
    }

    // public async Task<PaginationResponse<TResult>> PaginatedListAsync<TResult>(SearchDescriptor<T> query, QueryRequest request)
    // {
    //     SearchDescriptor<T> search = query
    //         .Sort(request)
    //         .Skip((request.CurrentPage - 1) * request.Size)
    //         .Take(request.Size)
    //         .Index(indexName);

    //     ISearchResponse<T> searchResponse = await elasticClient.SearchAsync<T>(search);

    //     return new PaginationResponse<TResult>(
    //         mapper.Map<IEnumerable<TResult>>(searchResponse.Documents.AsEnumerable()),
    //         (int)searchResponse.Total,
    //         request.CurrentPage,
    //         request.Size);
    // }

    public async Task UpdateAsync(T entity)
    {
        await elasticClient.UpdateAsync<T, T>(
            entity,
            i => i.Doc(entity).Index(indexName).Refresh(Refresh.WaitFor)
        );
    }

    public async Task UpdateByQueryAsync(
        T entity,
        string query,
        Dictionary<string, object> parameters
    )
    {
        await elasticClient.UpdateAsync<T, T>(
            entity,
            u =>
                u.Index(indexName)
                    .Script(s =>
                        s.Source(query)
                            .Params(param => new FluentDictionary<string, object>(parameters))
                    )
                    .Refresh(Refresh.True)
        );
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        await elasticClient.BulkAsync(x =>
            x.Index(indexName).UpdateMany(entities, (x, i) => x.Doc(i)).Refresh(Refresh.WaitFor)
        );
    }
}
