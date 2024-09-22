using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Services.Elastics;

public class ElasticsearchIndexBackgoundService(
    ILogger logger,
    ElasticsearchClient elasticsearchClient
) : IHostedLifecycleService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var configures = ElasticsearchRegisterHelper.GetElasticsearchConfigBuilder(
                Assembly.GetExecutingAssembly()
            );
            await ElasticsearchRegisterHelper.ElasticFluentConfigAsync(
                elasticsearchClient,
                configures
            );

            await ElasticsearchRegisterHelper.SeedingAsync(elasticsearchClient);
        }
        catch (Exception ex)
        {
            logger.Error("register elastic search index with err {message}", ex.Message);
        }
    }

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        logger.Information("StartedAsync");
        await Task.CompletedTask;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        logger.Information("StartingAsync");
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.Information("StopAsync");
        await Task.CompletedTask;
    }

    public async Task StoppedAsync(CancellationToken cancellationToken)
    {
        logger.Information("StoppedAsync");
        await Task.CompletedTask;
    }

    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        logger.Information("StoppedAsync");
        await Task.CompletedTask;
    }
}
