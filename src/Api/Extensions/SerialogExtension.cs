using System.Reflection;
using Api.Settings;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Api.Extensions;

public static class SerialogExtension
{
    public static void AddSerialogs(this WebApplicationBuilder web)
    {
        web.Host.UseSerilog((context, config) =>
        {
            ElasticsearchSettings? elasticsearch = context.Configuration.GetSection(nameof(ElasticsearchSettings)).Get<ElasticsearchSettings>();
            var sinksOption = new ElasticsearchSinkOptions(new Uri(elasticsearch!.Nodes[0]))
            {
                ModifyConnectionSettings = x => x.BasicAuthentication(elasticsearch.Username,elasticsearch.Password),
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                NumberOfReplicas = 1,
                NumberOfShards = 2,
            };

            config
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.Elasticsearch(sinksOption)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName!)
                .ReadFrom.Configuration(context.Configuration);
        });
    }
}