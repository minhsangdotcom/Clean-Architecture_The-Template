using Api.Settings;
using Serilog;
namespace Api.Extensions;

public static class SerialogExtension
{
    public static void AddSerialogsToOtelp(this WebApplicationBuilder web)
    {
        web.Host.UseSerilog(
            (context, config) =>
            {
                OpenTelemetrySettings? openTelemetrySettings = web
                    .Configuration.GetSection(nameof(OpenTelemetrySettings))
                    .Get<OpenTelemetrySettings>();

                config
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = openTelemetrySettings!.Otelp;
                        options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = openTelemetrySettings.ServiceName!,
                            ["Environment"] = context.HostingEnvironment.EnvironmentName,
                        };
                        options.IncludedData =
                            Serilog.Sinks.OpenTelemetry.IncludedData.TraceIdField
                            | Serilog.Sinks.OpenTelemetry.IncludedData.SpanIdField
                            | Serilog.Sinks.OpenTelemetry.IncludedData.MessageTemplateTextAttribute;
                    });
            }
        );
    }
}
