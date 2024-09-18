using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Domain.Common.ElasticConfigurations;

public class ElasticsearchConfiguration<T> where T : class
{
    public Expression<Func<T, object>>? DocumentId { get; set; }

    public Action<PropertiesDescriptor<T>>? Mapping { get; set; }

    public List<Expression<Func<T, object>>> IgnoreProperties { get; set; } = [];
    
    public string? IndexName { get; set; }
}