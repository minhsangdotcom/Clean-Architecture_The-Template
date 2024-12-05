using System.Diagnostics;
using Api.Settings;
using Contracts.Routers;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<OpenTelemetrySettings>(
            configuration.GetSection(nameof(OpenTelemetrySettings))
        );
        OpenTelemetrySettings openTelemetrySettings =
            configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()
            ?? new();

        if (openTelemetrySettings.IsEnabled)
        {
            ActivitySource source = new(openTelemetrySettings!.ActivitySourceName!);

            services
                .AddOpenTelemetry()
                .ConfigureResource(r =>
                    r.AddService(
                        serviceName: openTelemetrySettings!.ServiceName!,
                        serviceVersion: openTelemetrySettings.ServiceVersion ?? "unknown",
                        serviceInstanceId: Environment.MachineName
                    )
                )
                .WithTracing(options =>
                {
                    options
                        .AddSource(openTelemetrySettings!.ActivitySourceName!)
                        .AddHttpClientInstrumentation();

                    options.AddAspNetCoreInstrumentation(options =>
                    {
                        // to trace only api requests
                        options.Filter = (context) =>
                            !string.IsNullOrEmpty(context.Request.Path.Value)
                            && context.Request.Path.Value.Contains(
                                Router.prefix.Replace("/", string.Empty),
                                StringComparison.InvariantCulture
                            );

                        // enrich activity with http request and response
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                        };
                        options.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };

                        // automatically sets Activity Status to Error if an unhandled exception is thrown
                        options.RecordException = true;
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception?.GetType().ToString());
                            activity.SetTag("stackTrace", exception?.StackTrace);
                        };
                    });

                    options.AddEntityFrameworkCoreInstrumentation(opt =>
                    {
                        opt.SetDbStatementForText = true;
                        opt.SetDbStatementForStoredProcedure = true;
                        opt.EnrichWithIDbCommand = (activity, command) => {
                            // var stateDisplayName = $"{command.CommandType} main";
                            // activity.DisplayName = stateDisplayName;
                            // activity.SetTag("db.name", stateDisplayName);
                        };
                    });

                    switch (openTelemetrySettings.OtelpOption)
                    {
                        case OtelpOption.DistributedServer:
                            options.AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri(openTelemetrySettings.Otelp!.ToString());
                                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                options.TimeoutMilliseconds = 300000;
                            });
                            break;
                        case OtelpOption.Console:
                            options.AddConsoleExporter();
                            break;
                        default:
                            break;
                    }
                });
        }

        return services;
    }
}
