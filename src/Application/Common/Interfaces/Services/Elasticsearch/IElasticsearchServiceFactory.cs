namespace Application.Common.Interfaces.Services.Elasticsearch;

public interface IElasticsearchServiceFactory
{
    IElasticsearchService<TEntity> Get<TEntity>()
        where TEntity : class;
}
