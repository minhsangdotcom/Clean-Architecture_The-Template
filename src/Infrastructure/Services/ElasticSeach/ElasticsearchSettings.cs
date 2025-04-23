namespace Infrastructure.Services.ElasticSeach;

public class ElasticsearchSettings
{
    public List<string> Nodes { get; set; } = [];

    public string? Username { get; set; }

    public string? Password { get; set; }

    /// <summary>
    /// Dicide to use elasticsearch or not
    /// </summary>
    public bool IsEnbaled { get; set; }

    public string? DefaultIndex { get; set; } = "DefaultIndex";

    public string? PrefixIndex { get; set; } = "TheTemplate";
}
