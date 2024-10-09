using Application.Common.Interfaces.Registers;

namespace Application.Common.Interfaces.Services.Elastics;

public interface IElasticsearchServiceFactory : ISingleton
{
    IElasticsearchService<TEntity> Get<TEntity>() where TEntity : class;
}
