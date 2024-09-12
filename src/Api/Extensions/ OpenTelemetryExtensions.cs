using System.Diagnostics;
using Api.Settings;
using Contracts.Routers;
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
        var openTelemetrySettings = configuration
            .GetSection(nameof(OpenTelemetrySettings))
            .Get<OpenTelemetrySettings>();

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

                options.AddAspNetCoreInstrumentation(o =>
                {
                    // to trace only api requests
                    o.Filter = (context) =>
                        !string.IsNullOrEmpty(context.Request.Path.Value)
                        && context.Request.Path.Value.Contains(
                            Router.prefix.Replace("/", string.Empty),
                            StringComparison.InvariantCulture
                        );

                    // enrich activity with http request and response
                    o.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("requestProtocol", httpRequest.Protocol);
                    };
                    o.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        activity.SetTag("responseLength", httpResponse.ContentLength);
                    };

                    // automatically sets Activity Status to Error if an unhandled exception is thrown
                    o.RecordException = true;
                    o.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exceptionType", exception.GetType().ToString());
                        activity.SetTag("stackTrace", exception.StackTrace);
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

                options.AddConsoleExporter();
                //options.AddOtlpExporter();
            });

        return services;
    }
}
