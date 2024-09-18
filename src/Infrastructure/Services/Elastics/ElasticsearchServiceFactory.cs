using Application.Common.Interfaces.Services.Elastics;
using Elastic.Clients.Elasticsearch;

namespace Infrastructure.Services.Elastics;

public class ElasticsearchServiceFactory(ElasticsearchClient elasticClient)
    : IElasticsearchServiceFactory
{
    private readonly Dictionary<string, object?> repositories = [];

    public IElasticsearchService<TEntity> Get<TEntity>()
        where TEntity : class
    {
        string type = typeof(TEntity).Name;

        if (!repositories.TryGetValue(type, out object? value))
        {
            Type repositoryType = typeof(ElasticsearchService<>);
            object? repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)),
                [elasticClient]
            );
            value = repositoryInstance;
            repositories.Add(type, value);
        }

        return (IElasticsearchService<TEntity>)value!;
    }
}
