namespace Api.Settings;

public class OpenTelemetrySettings
{
    public string? ServiceName { get; set; } = "Testing Service";
    public string? ServiceVersion { get; set; } = "1.0.0";
    public string? ActivitySourceName { get; set; } = "Testing Service";

    public string? Otelp { get; set; }

    public OtelpOption OtelpOption { get; set; } = OtelpOption.Non;

    public bool IsEnabled { get; set; }
}

public enum OtelpOption
{
    /// <summary>
    /// Add log to distributed server
    /// </summary>
    DistributedServer = 1,

    /// <summary>
    /// Add log to console
    /// </summary>
    Console = 2,

    /// <summary>
    /// Do not thing
    /// </summary>
    Non = 3,
}
