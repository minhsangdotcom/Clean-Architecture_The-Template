using System.Reflection;
using Application.Common.Interfaces.Services.Elastics;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Elastics;

public static class ElasticSearchExtension
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ElasticsearchSettings elasticsearch =
            configuration.GetSection(nameof(ElasticsearchSettings)).Get<ElasticsearchSettings>()
            ?? new();

        if (elasticsearch.IsEnbaled)
        {
            IEnumerable<Uri> nodes = elasticsearch!.Nodes.Select(x => new Uri(x));
            var pool = new StaticNodePool(nodes);
            string? userName = elasticsearch.Username;
            string? password = elasticsearch.Password;

            var settings = new ElasticsearchClientSettings(pool).DefaultIndex(
                elasticsearch.DefaultIndex!
            );

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                settings.Authentication(new BasicAuthentication(userName, password));
            }

            IEnumerable<ElasticConfigureResult> elkConfigbuilder =
                ElasticsearchRegisterHelper.GetElasticsearchConfigBuilder(
                    Assembly.GetExecutingAssembly()
                );
            ElasticsearchRegisterHelper.ConfigureConnectionSettings(ref settings, elkConfigbuilder);

            var client = new ElasticsearchClient(settings);

            services
                .AddSingleton(client)
                .AddHostedService<ElasticsearchIndexBackgoundService>()
                .AddSingleton<IElasticsearchServiceFactory, ElasticsearchServiceFactory>();
        }

        return services;
    }
}
