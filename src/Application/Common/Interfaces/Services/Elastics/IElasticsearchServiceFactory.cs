namespace Application.Common.Interfaces.Services.Elastics;

public interface IElasticsearchServiceFactory
{
    IElasticsearchService<TEntity> Get<TEntity>() where TEntity : class;
}
