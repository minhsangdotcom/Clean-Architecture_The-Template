namespace Infrastructure.Services.Elastics;

public class ElasticsearchSettings
{
    public List<string>  Nodes { get; set; } = [];

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool IsEnable { get; set; }
}