using System.Reflection;
using Application.Common.Interfaces.Services.Elastics;
using Contracts.Extensions;
using Domain.Aggregates.AuditLogs;
using Domain.Aggregates.AuditLogs.Enums;
using Domain.Aggregates.Users.Enums;
using Domain.Common.ElasticConfigurations;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Services.Elastics;

public static class ElasticSearchExtension
{
    public static async Task<IServiceCollection> AddElasticSearchAsync(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ElasticsearchSettings? elasticsearch = configuration
            .GetSection(nameof(ElasticsearchSettings))
            .Get<ElasticsearchSettings>();

        if (elasticsearch?.IsEnable == true)
        {
            IEnumerable<Uri> nodes = elasticsearch!.Nodes.Select(x => new Uri(x));
            var pool = new StaticNodePool(nodes);
            string? userName = elasticsearch.Username;
            string? password = elasticsearch.Password;

            var settings = new ElasticsearchClientSettings(pool).DefaultIndex("default_index");

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                settings.Authentication(new BasicAuthentication(userName, password));
            }

            Assembly currentAssemply = Assembly.GetExecutingAssembly();
            IEnumerable<ElsConfig> elkConfigbuilder = GetElasticsearchConfigBuilder(
                currentAssemply
            );
            ConfigureConnectionSettings(ref settings, elkConfigbuilder);

            var client = new ElasticsearchClient(settings);
            await ConfigureElasticClient(client, elkConfigbuilder);
            await Seeding(client);

            services
                .AddSingleton(client)
                .AddSingleton<IElasticsearchServiceFactory, ElasticsearchServiceFactory>();
        }

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

    private static async Task Seeding(ElasticsearchClient elasticsearchClient)
    {
        var auditLog = await elasticsearchClient.SearchAsync<AuditLog>();
        if (auditLog.Documents.Count > 0)
        {
            return;
        }
        string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<AuditLog> auditLogs = [];
        for (int i = 0; i < 10; i++)
        {
            string entity =
                $"{StringExtension.GenerateRandomString(4, allowedChars)} {StringExtension.GenerateRandomString(4, allowedChars)} {i}";

            int[] types = Enum.GetValues(typeof(AuditLogType)).Cast<int>().ToArray();
            int index = new Random().Next(0, types.Length - 1);

            auditLogs.Add(
                new()
                {
                    Id = Ulid.NewUlid().ToString(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    Entity = entity,
                    Type = (AuditLogType)index,
                    ActionPerformBy = Ulid.NewUlid().ToString(),
                    Agent = new()
                    {
                        Id = Ulid.NewUlid().ToString(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        Gender = (Gender)new Random().Next(0, 1),
                        FirstName = $"{StringExtension.GenerateRandomString(4, allowedChars)} {i}",
                        LastName = $"{StringExtension.GenerateRandomString(4, allowedChars)} {i}",
                        Email = $"anna.kim{i}@gmail.com",
                        DayOfBirth = new DateTime(1990, 1, 1 + i),
                    },
                }
            );
        }

        BulkResponse response = await elasticsearchClient.BulkAsync(b =>
            b.Index(ElsIndexExtension.GetName<AuditLog>())
                .CreateMany(auditLogs)
                .Refresh(Refresh.WaitFor)
        );

        if (response.IsSuccess())
        {
            Log.Information("Elasticsearch has seeded.");
        }
        else
        {
            Log.Information(
                "Elasticsearch has been failed in seeding with {debug}",
                response.DebugInformation
            );
        }
    }
}

internal record ElsConfig(object Configs, Type Type);
