using Api.Converters;
using Api.Extensions;
using Application;
using Contracts.Binds;
using Infrastructure;
using Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// ---------------------------------------------
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

services.AddInfrastructureServices(configuration);
services.AddApplicationServices();

try
{
    Log.Logger.Information("Application is starting....");
    var app = builder.Build();

    var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    await DbInitializer.InitializeAsync(serviceProvider);
    await RegionDataSeeding.SeedingAsync(serviceProvider);

    if (app.Environment.IsDevelopment())
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

    Log.Logger.Information("Application is launching");
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
