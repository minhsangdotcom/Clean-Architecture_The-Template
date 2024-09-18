using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Domain.Common.ElasticConfigurations;

public class ElasticsearchConfigBuilder<T> where T : class
{
    public ElasticsearchConfiguration<T> Configuration => configuration;

    private readonly ElasticsearchConfiguration<T> configuration = new();

    public ElasticsearchConfigBuilder<T> HasKey(Expression<Func<T, object>> DocumentId)
    {
        configuration.DocumentId = DocumentId;
        return this;
    }

    public ElasticsearchConfigBuilder<T> Properties(Action<PropertiesDescriptor<T>> configure)
    {
        configuration.Mapping = configure;
        return this;
    }

    public ElasticsearchConfigBuilder<T> ToIndex()
    {
        configuration.IndexName = ElsIndexExtension.GetName<T>();
        return this;
    }

    public ElasticsearchConfigBuilder<T> Ignores(List<Expression<Func<T, object>>> ignoreProperties)
    {
        configuration.IgnoreProperties = ignoreProperties;
        return this;
    }
}