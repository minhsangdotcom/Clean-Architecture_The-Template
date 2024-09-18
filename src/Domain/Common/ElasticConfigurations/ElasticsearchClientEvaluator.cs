using System.Dynamic;
using Ardalis.GuardClauses;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        var mappingProperties = builder.Configuration.Mapping;

        int deep = GetNestedDeep(typeof(TEntity));

        if (!(await elasticsearchClient.Indices.ExistsAsync(indexName)).Exists)
        {
            void Configure(TypeMappingDescriptor typeMapping)
            {
                if (mappingProperties != null)
                {
                    typeMapping.Properties(mappingProperties);
                }
            }

            void ConfigureRequest(CreateIndexRequestDescriptor descriptor) =>
                descriptor.Mappings(Configure);

            CreateIndexResponse response = await elasticsearchClient.Indices.CreateAsync(
                indexName,
                ConfigureRequest
            );

            if (response.IsSuccess())
            {
                Console.WriteLine($"create elasticsearch {indexName} index mapping.");
            }
            else
            {
                Console.WriteLine(
                    $"create elasticsearch {indexName} index mapping has failed with {response.DebugInformation}"
                );
            }

            return;
        }

        // GetMappingResponse? mappingResponse = await elasticsearchClient.Indices.GetMappingAsync<StringResponse>(
        //     indexName
        // );

        // object? elsMapping = JsonConvert.DeserializeObject<ExpandoObject>(mappingResponse);

        // string body = JsonConvert.SerializeObject(
        //     ((IDictionary<string, object>)elsMapping!).FirstOrDefault().Value
        // );

        // ElasticsearchMappingSettings? elasticsearchMapping =
        //     JsonConvert.DeserializeObject<ElasticsearchMappingSettings>(body);

        // JObject? m = (JObject)elasticsearchMapping?.Mappings?.Properties!;

        // int mappingCount = CountMappingProperties(m);

        // int propertyCount = CountCurrentTypeProps(typeof(TEntity));

        // int currentPropslength = propertyCount - builder.Configuration.IgnoreProperties.Count;

        // if (currentPropslength > propertyCount)
        // {
        //     Func<PutMappingDescriptor<TEntity>, IPutMappingRequest> updateMapping =
        //         mappingProperties != null
        //             ? x => x.Index(indexName).AutoMap().Properties(mappingProperties)
        //             : x => x.Index(indexName).AutoMap(deep);

        //     elasticsearchClient.Indices.PutMapping(updateMapping);

        //     Console.WriteLine($"Update elasticsearch {indexName} index mapping.");
        // }
    }

    private static int CountMappingProperties(JObject m, int count = 0)
    {
        IEnumerable<JProperty> properties = m?.Properties() ?? new List<JProperty>();

        if (!properties.Any())
        {
            return count;
        }

        foreach (var item in properties)
        {
            //Console.WriteLine(item.Name);

            if (item.Value["type"]?.ToString() == "nested" || item.Value["properties"] != null)
            {
                JObject nestedProperty = item.Value["properties"]!.ToObject<JObject>()!;

                count = CountMappingProperties(nestedProperty, count++);
            }

            count++;
        }

        return count;
    }

    private static int CountCurrentTypeProps(Type type, int count = 0)
    {
        // foreach (var property in type.GetProperties())
        // {
        //     //Console.WriteLine(property.Name);

        //     if (property.IsUserDefineType())
        //     {
        //         count = CountCurrentTypeProps(property.PropertyType, count++);
        //     }

        //     count++;
        // }

        return count;
    }

    private static int GetNestedDeep(Type type, int count = 0)
    {
        if (type.GetProperties().Count() > 0)
        {
            count++;
        }

        var propertyInfo = type.GetProperties()
            .FirstOrDefault(propertyInfo =>
                propertyInfo.PropertyType.IsClass
                && !propertyInfo.PropertyType.FullName!.StartsWith("System.")
            );

        if (propertyInfo == null)
        {
            return count;
        }

        return GetNestedDeep(propertyInfo.PropertyType, count);
    }
}
