using System.Runtime.InteropServices;
using Api.Converters;
using Api.Extensions;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services.Hangfires;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region main dependencies
builder.AddConfiguration();
builder
    .Services.AddControllers()
    .AddJsonOptions(option =>
    {
        option.JsonSerializerOptions.Converters.Add(new DatetimeConverter());
        option.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConvert());
        option.JsonSerializerOptions.Converters.Add(
            new Cysharp.Serialization.Json.UlidJsonConverter()
        );
    });
services.AddSwagger(configuration);
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
    app.MapHealthChecks(
        "/api/health",
        new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
    );

    bool isDevelopment = app.Environment.IsDevelopment();

    #region seeding area
    if (
        app.Environment.EnvironmentName != "Testing-Deployment"
        && app.Environment.EnvironmentName != "Testing-Development"
    )
    {
        var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        await RegionDataSeeding.SeedingAsync(serviceProvider);
        await DbInitializer.InitializeAsync(serviceProvider);
    }
    #endregion

    app.UseHangfireDashboard(configuration);

    if (isDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            x.RoutePrefix = "docs";
            x.ConfigObject.PersistAuthorization = true;
        });
    }

    app.UseAuthentication();
    app.CurrentUser();
    app.UseAuthorization();
    app.UseDetection();

    app.UseSerilogRequestLogging();
    app.LogContext();
    app.ExceptionHandler();
    app.MapControllers();

    Log.Logger.Information(
        "Application is launching with {environment}",
        app.Environment.EnvironmentName
    );
    Log.Logger.Information("Application is running on {os}", RuntimeInformation.OSDescription);
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
