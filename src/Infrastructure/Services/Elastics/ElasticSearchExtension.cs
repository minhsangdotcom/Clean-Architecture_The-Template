using System.Reflection;
using Domain.Common.ElasticConfigurations;
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
        ElasticsearchSettings? elasticsearch = configuration
            .GetSection(nameof(ElasticsearchSettings))
            .Get<ElasticsearchSettings>();

        IEnumerable<Uri> nodes = elasticsearch!.Nodes.Select(x => new Uri(x));

        var pool = new StaticNodePool(nodes);

        string? userName = elasticsearch.Username;
        string? password = elasticsearch.Password;

        var settings = new ElasticsearchClientSettings(pool).DefaultIndex("default_index");

        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
        {
            settings.Authentication(new BasicAuthentication(userName, password));
        }

        var currentAssemply = Assembly.GetExecutingAssembly();
        IEnumerable<ElsConfig> elkConfigbuilder = GetElasticsearchConfigBuilder(currentAssemply!);
        ConfigureConnectionSettings(ref settings, elkConfigbuilder);

        var client = new ElasticsearchClient(settings);
        ConfigureElasticClient(client, elkConfigbuilder).GetAwaiter();
        //client.DataSeeding();

        services.AddSingleton(client);
        return services;
    }

    private static IEnumerable<ElsConfig> GetElasticsearchConfigBuilder(Assembly assembly)
    {
        var configuringTypes = assembly
            .GetTypes()
            .Where(x =>
                x.GetInterfaces()
                    .Any(p =>
                        p.IsGenericType
                        && p.GetGenericTypeDefinition() == typeof(IElasticsearchDocumentConfigure<>)
                    )
            )
            .Select(x => new
            {
                type = x,
                iType = x.GetInterfaces()
                    .FirstOrDefault(p =>
                        p.GetGenericTypeDefinition() == typeof(IElasticsearchDocumentConfigure<>)
                    )!
                    .GenericTypeArguments[0],
            });

        foreach (var configuringType in configuringTypes)
        {
            MethodInfo? method = configuringType.type.GetMethod(
                nameof(IElasticsearchDocumentConfigure<object>.Configure)
            );

            if (method == null)
            {
                continue;
            }

            object? elasticsearchConfigBuilder = Activator.CreateInstance(
                typeof(ElasticsearchConfigBuilder<>).MakeGenericType(configuringType.iType!)
            );

            object? elsConfig = Activator.CreateInstance(configuringType.type);

            method!.Invoke(elsConfig, [elasticsearchConfigBuilder!]);

            yield return new ElsConfig(elasticsearchConfigBuilder!, configuringType.iType);
        }
    }

    // * config map setting
    private static void ConfigureConnectionSettings(
        ref ElasticsearchClientSettings connectionSettings,
        IEnumerable<ElsConfig> elsConfigs
    )
    {
        foreach (var elsConfig in elsConfigs)
        {
            object? connectionSettingEvaluator = Activator.CreateInstance(
                typeof(ConnectionSettingEvaluator),
                [connectionSettings]
            );

            var evaluateMethodInfo = typeof(ConnectionSettingEvaluator)
                .GetMethod(nameof(IEvaluator.Evaluate))!
                .MakeGenericMethod(elsConfig.Type);

            (
                (Task)evaluateMethodInfo.Invoke(connectionSettingEvaluator, [elsConfig.Configs])!
            ).GetAwaiter();
        }
    }

    // * config map property
    private static async Task ConfigureElasticClient(
        ElasticsearchClient elasticClient,
        IEnumerable<ElsConfig> elsConfigs
    )
    {
        foreach (var elsConfig in elsConfigs)
        {
            object? elasticsearchClientEvaluator = Activator.CreateInstance(
                typeof(ElasticsearchClientEvaluator),
                [elasticClient]
            );

            var evaluateMethodInfo = typeof(ElasticsearchClientEvaluator)
                .GetMethod(nameof(IEvaluator.Evaluate))!
                .MakeGenericMethod(elsConfig.Type);

            await (Task)
                evaluateMethodInfo.Invoke(elasticsearchClientEvaluator, [elsConfig.Configs])!;
        }
    }
}

internal record ElsConfig(object Configs, Type Type);
