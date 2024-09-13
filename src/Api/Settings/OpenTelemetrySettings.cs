namespace Api.Settings;

public class OpenTelemetrySettings
{
    public string? ServiceName { get; set; }
    public string? ServiceVersion { get; set; }
    public string? ActivitySourceName { get; set; }

    public string? Otelp { get; set; }

    public OtelpOption OtelpOption { get; set; } = OtelpOption.Non;
}

public enum OtelpOption
{
    Active = 1,
    Inactive = 2,
    Non = 3,
}
