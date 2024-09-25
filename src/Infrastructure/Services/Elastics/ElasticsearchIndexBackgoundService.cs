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
            if (!(await elasticsearchClient.PingAsync(cancellationToken)).IsSuccess())
            {
                return;
            }

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
        await Task.CompletedTask;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StoppedAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
