using Api.Converters;
using Api.Extensions;
using Api.Middlewares;
using Application;
using Infrastructure;
using Infrastructure.Data;

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
services.AddSwagger();

//-----------------------------

services.AddInfrastructureServices(configuration);
services.AddApplicationServices();

var app = builder.Build();
app.Logger.LogInformation("Application is starting....");

var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

DbInitializer.Initialize(serviceProvider).GetAwaiter();

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

app.ExceptionHandler();
app.MapControllers();

app.Logger.LogInformation("Application is launching");
app.Run();
