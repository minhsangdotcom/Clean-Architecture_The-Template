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
    DistributedServer = 1,
    Console = 2,
    Non = 3,
}
