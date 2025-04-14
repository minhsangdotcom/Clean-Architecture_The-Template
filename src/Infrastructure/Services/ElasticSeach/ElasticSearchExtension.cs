using System.Reflection;
using Application.Common.Interfaces.Services.Elastics;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FluentConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.ElasticSeach;

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
        services.AddSingleton(typeof(IElasticsearchService<>), typeof(ElasticsearchService<>));

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
                settings
                    .Authentication(new BasicAuthentication(userName, password))
                    // without ssl trust
                    .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
                    .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
            }

            IEnumerable<ElasticConfigureResult> elkConfigbuilder =
                ElasticsearchRegisterHelper.GetElasticsearchConfigBuilder(
                    Assembly.GetExecutingAssembly(),
                    elasticsearch.PrefixIndex!
                );

            // add configurations of id, ignore properties
            ElasticsearchRegisterHelper.ConfigureConnectionSettings(ref settings, elkConfigbuilder);

            var client = new ElasticsearchClient(settings);

            ElasticsearchRegisterHelper
                .ElasticFluentConfigAsync(client, elkConfigbuilder)
                .ConfigureAwait(false)
                .GetAwaiter();

            DataSeeding
                .SeedingAsync(client, elasticsearch.PrefixIndex)
                .ConfigureAwait(false)
                .GetAwaiter();

            services
                .AddSingleton(client)
                .AddSingleton<IElasticsearchServiceFactory, ElasticsearchServiceFactory>();
        }

        return services;
    }
}
