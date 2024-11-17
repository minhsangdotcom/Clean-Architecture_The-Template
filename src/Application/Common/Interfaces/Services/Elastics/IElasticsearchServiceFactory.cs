using Application.Common.Interfaces.Registers;

namespace Application.Common.Interfaces.Services.Elastics;

public interface IElasticsearchServiceFactory
{
    IElasticsearchService<TEntity> Get<TEntity>() where TEntity : class;
}
