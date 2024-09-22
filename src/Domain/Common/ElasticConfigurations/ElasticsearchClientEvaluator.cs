using Ardalis.GuardClauses;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport.Products.Elasticsearch;

namespace Domain.Common.ElasticConfigurations;

public class ElasticsearchClientEvaluator(ElasticsearchClient elasticsearchClient) : IEvaluator
{
    public async Task Evaluate<TEntity>(ElasticsearchConfigBuilder<TEntity> builder)
        where TEntity : class
    {
        string indexName = Guard.Against.Null(
            builder.Configuration.IndexName,
            nameof(builder.Configuration.IndexName),
            "Missing index name."
        );

        Action<PropertiesDescriptor<TEntity>> maps = Guard.Against.Null(
            builder.Configuration.Mapping,
            nameof(builder.Configuration.Mapping),
            "Missing mapping properties."
        );

        await CreateIndexOrPutMappingAsync(indexName, maps);
    }

    private async Task CreateIndexOrPutMappingAsync<TEntity>(
        string indexName,
        Action<PropertiesDescriptor<TEntity>> properties
    )
        where TEntity : class
    {
        var existsResponse = await elasticsearchClient.Indices.ExistsAsync(indexName);

        ElasticsearchResponse elasticsearchResponse = !existsResponse.Exists
            ? await elasticsearchClient.Indices.CreateAsync(
                indexName,
                config => config.Mappings(typeMap => typeMap.Properties(properties))
            )
            : await elasticsearchClient.Indices.PutMappingAsync(
                indexName,
                config => config.Properties<TEntity>(properties)
            );

        string action = !existsResponse.Exists ? "Create" : "Update";

        if (elasticsearchResponse.IsSuccess())
        {
            Console.WriteLine($@"{action} elasticsearch {indexName} index sucessfully!");
        }
        else
        {
            Console.WriteLine(
                $"{action} elasticsearch {indexName} index mapping has failed with {elasticsearchResponse.DebugInformation}"
            );
        }
    }
}
