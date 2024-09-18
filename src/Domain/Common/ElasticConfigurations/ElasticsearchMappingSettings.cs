namespace Domain.Common.ElasticConfigurations;

public class ElasticsearchMappingSettings
{
    public ElsMappings? Mappings { get; set; }
}

public class ElsMappings
{
    public object? Properties { get; set; }
}