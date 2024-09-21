using Elastic.Clients.Elasticsearch;

namespace Domain.Common.ElasticConfigurations;

public class ConnectionSettingEvaluator(ElasticsearchClientSettings settings) : IEvaluator
{
    public async Task Evaluate<TEntity>(ElasticsearchConfigBuilder<TEntity> builder)
        where TEntity : class
    {
        void Selector(ClrTypeMappingDescriptor<TEntity> descriptor)
        {
            ClrTypeMappingDescriptor<TEntity> result = null!;

            if (builder.Configuration.DocumentId != null)
            {
                result =
                    result == null
                        ? descriptor.IdProperty(builder.Configuration.DocumentId)
                        : result.IdProperty(builder.Configuration.DocumentId);
            }
        }

        settings.DefaultMappingFor((Action<ClrTypeMappingDescriptor<TEntity>>)Selector);
        await Task.CompletedTask;
    }
}
