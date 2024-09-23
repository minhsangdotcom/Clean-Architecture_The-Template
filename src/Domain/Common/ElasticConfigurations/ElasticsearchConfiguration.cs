using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.TransformManagement;

namespace Domain.Common.ElasticConfigurations;

public class ElasticsearchConfiguration<T> where T : class
{
    public Expression<Func<T, object>>? DocumentId { get; set; }

    public Action<PropertiesDescriptor<T>>? Mapping { get; set; }

    public Action<IndexSettingsDescriptor>? Settings { get; set; }

    public List<Expression<Func<T, object>>> IgnoreProperties { get; set; } = [];
    
    public string? IndexName { get; set; }
}