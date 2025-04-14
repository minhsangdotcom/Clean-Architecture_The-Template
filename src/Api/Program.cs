using System.Runtime.InteropServices;
using Api.common.EndpointConfigurations;
using Api.common.Routers;
using Api.Converters;
using Api.Extensions;
using Application;
using Cysharp.Serialization.Json;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services.Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region main dependencies
string? url = builder.Configuration["urls"] ?? "http://0.0.0.0:8080";
builder.WebHost.UseUrls(url);
builder.AddConfiguration();

services.AddEndpoints();
services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new DatetimeConverter());
    options.SerializerOptions.Converters.Add(new DateTimeOffsetConvert());
    options.SerializerOptions.Converters.Add(new UlidJsonConverter());
});

services.AddAuthorization();
services.AddErrorDetails();
services.AddSwagger(configuration);
services.AddApiVersion();
services.AddOpenTelemetryTracing(configuration);
builder.AddSerialogs();
services.AddHealthChecks();
services.AddDatabaseHealthCheck(configuration);
#endregion

#region layers dependencies
services.AddInfrastructureDependencies(configuration, builder.Environment.EnvironmentName);
services.AddApplicationDependencies();
#endregion

try
{
    Log.Logger.Information("Application is starting....");
    var app = builder.Build();

    string healthCheckPath =
        configuration.GetSection("HealthCheckPath").Get<string>() ?? "/api/health";
    app.MapHealthChecks(
        healthCheckPath,
        new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
    );

    bool isDevelopment = app.Environment.IsDevelopment();

    #region seeding area
    if (
        app.Environment.EnvironmentName != "Testing-Deployment"
        && app.Environment.EnvironmentName != "Testing-Development"
    )
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        await RegionDataSeeding.SeedingAsync(serviceProvider);
        await DbInitializer.InitializeAsync(serviceProvider);
    }
    #endregion

    app.UseHangfireDashboard(configuration);

    const string routeRefix = "docs";
    if (isDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI(configs =>
        {
            configs.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            configs.RoutePrefix = routeRefix;
            configs.ConfigObject.PersistAuthorization = true;
            configs.DocExpansion(DocExpansion.None);
        });
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var server = app.Services.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses.ToArray();

            if (addresses != null && addresses.Length > 0)
            {
                string? url = addresses?[0];
                string? renewUrl =
                    url?.Contains("0.0.0.0") == true ? url.Replace("0.0.0.0", "localhost") : url;
                Log.Logger.Information("Application is running at: {Url}", renewUrl);
                Log.Logger.Information(
                    "Swagger UI is running at: {Url}",
                    $"{renewUrl}/{routeRefix}"
                );
                Log.Logger.Information(
                    "Application health check is running at: {Url}",
                    $"{renewUrl}{healthCheckPath}"
                );
            }
        });
    }

    app.UseStatusCodePages();
    app.UseExceptionHandler();
    app.UseAuthentication();
    app.CurrentUser();
    app.UseAuthorization();
    app.UseDetection();

    app.MapEndpoints(apiVersion: EndpointVersion.One);

    Log.Logger.Information(
        "Application is in {environment} environment",
        app.Environment.EnvironmentName
    );
    Log.Logger.Information("Application is hosted on {os}", RuntimeInformation.OSDescription);
    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Fatal("Application has launched fail with error {error}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
