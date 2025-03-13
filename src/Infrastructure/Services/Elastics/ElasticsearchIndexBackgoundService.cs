using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Services.Elastics;

public class ElasticsearchIndexBackgoundService(
    ILogger logger,
    ElasticsearchClient elasticsearchClient,
    IOptions<ElasticsearchSettings> options
) : IHostedLifecycleService
{
    private readonly ElasticsearchSettings elasticsearchSettings = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!(await elasticsearchClient.PingAsync(cancellationToken)).IsSuccess())
            {
                logger.Warning("Cannot connect elasticsearch server");
                return;
            }

            var configures = ElasticsearchRegisterHelper.GetElasticsearchConfigBuilder(
                Assembly.GetExecutingAssembly(),
                elasticsearchSettings.PrefixIndex!
            );
            await ElasticsearchRegisterHelper.ElasticFluentConfigAsync(
                elasticsearchClient,
                configures
            );

            await ElasticsearchRegisterHelper.SeedingAsync(
                elasticsearchClient,
                elasticsearchSettings.PrefixIndex!
            );
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
