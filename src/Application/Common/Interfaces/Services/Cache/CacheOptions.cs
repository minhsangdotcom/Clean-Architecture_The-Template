namespace Application.Common.Interfaces.Services.Cache;

public enum CacheExpirationType
{
    None, // no expiration
    Absolute, // expires after a fixed time
    Sliding, // expiry resets on each access
}

public class CacheOptions
{
    /// <summary>
    /// Which expiration strategy to use.
    /// Defaults to Absolute.
    /// </summary>
    public CacheExpirationType ExpirationType { get; set; } = CacheExpirationType.Absolute;

    /// <summary>
    /// Time span to use for either absolute or sliding expiration.
    /// Ignored if ExpirationType == None.
    /// If null, you can fall back to a global default in your implementation.
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    // (Optional) You could add more fields here:
    // public DateTimeOffset? AbsoluteExpiration { get; set; }
    // public Action<string, object, EvictionReason, TimeSpan> PostEvictionCallback { get; set; }
}
