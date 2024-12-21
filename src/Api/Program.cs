using Api.Converters;
using Api.Extensions;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services.Hangfires;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// ---------------------------------------------
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

//-----------------------------

services.AddInfrastructureServices(configuration, builder.Environment.EnvironmentName);
services.AddApplicationServices();

try
{
    Log.Logger.Information("Application is starting....");
    var app = builder.Build();

    bool isDevelopment = app.Environment.IsDevelopment();
    if (isDevelopment)
    {
        var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        await RegionDataSeeding.SeedingAsync(serviceProvider);
        await DbInitializer.InitializeAsync(serviceProvider);
    }

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
