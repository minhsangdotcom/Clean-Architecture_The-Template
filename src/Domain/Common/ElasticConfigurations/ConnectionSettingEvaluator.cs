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
            // if (builder.Configuration.IgnoreProperties.Count > 0)
            // {
            //     var ignoreProperties = builder.Configuration.IgnoreProperties;

            //     for (int i = 0; i < ignoreProperties.Count; i++)
            //     {
            //         var property = ignoreProperties[i];

            //         result = result == null && i == 0 ?
            //             x.Ignore(property) :
            //                 result!.Ignore(property);
            //     }
            // }

            //return result ??= x;
        }
        //settings.DefaultMappingFor((Func<ClrTypeMappingDescriptor<TEntity>, IClrTypeMapping<TEntity>>)selector);

        settings.DefaultMappingFor((Action<ClrTypeMappingDescriptor<TEntity>>)Selector);

        await Task.CompletedTask;
    }
}
