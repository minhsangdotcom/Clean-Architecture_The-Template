namespace Infrastructure.Services.DistributedCache;

public class RedisDatabaseSettings
{
    public string? Host { get; set; }

    public int? Port { get; set; }

    public string? Password { get; set; }

    public bool IsEnbaled { get; set; }
}