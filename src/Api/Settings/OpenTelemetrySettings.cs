namespace Api.Settings;

public class OpenTelemetrySettings
{
    public string? ServiceName { get; set; }
    public string? ServiceVersion { get; set; }
    public string? ActivitySourceName { get; set; }
}
