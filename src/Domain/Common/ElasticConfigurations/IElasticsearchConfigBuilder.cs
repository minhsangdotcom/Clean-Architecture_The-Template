namespace Domain.Common.ElasticConfigurations;

public interface IElasticsearchDocumentConfigure<T> where T : class
{
    void Configure(ref ElasticsearchConfigBuilder<T> buider);
}