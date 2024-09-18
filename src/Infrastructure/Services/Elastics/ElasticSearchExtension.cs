using System.Reflection;
using Domain.Common;
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

        var currentAssemply = Assembly.GetAssembly(typeof(BaseEntity));
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

    // private static void DataSeeding(this ElasticClient elasticClient)
    // {
    //     string indexName = ElsIndexExtension.GetName<ElkMedia>();
    //     var res = elasticClient.Search<ElkMedia>(s => s.Query(q => q.MatchAll()).Size(1).Index(indexName));

    //     if (res.Documents.Count > 0)
    //     {
    //         return;
    //     }

    //     List<ElkMedia> medias = new();

    //     long size = 1234455;

    //     var types = new[]
    //     {
    //         new { ContentType = "video/mp4", Extension = ".mp4" },
    //         new { ContentType = "audio/mp3", Extension = ".mp3" },
    //         new { ContentType = "image/jpg", Extension = ".jpg" },
    //     };

    //     Random random = new();

    //     for (int i = 1; i <= 1000; i++)
    //     {
    //         int randomNumber = random.Next(0, 3);

    //         var type = types[randomNumber];

    //         string extension = type.Extension;

    //         string fileName = $"{type.ContentType[..type.ContentType.IndexOf('/')]}{i}{extension}";

    //         size += 1;

    //         var media = new ElkMedia()
    //         {
    //             Name = $"media{i}",
    //             ContentType = type.ContentType,
    //             ResourcePath = $"https://www.abc.com.vn/files/{fileName}",
    //             Extension = extension,
    //             Size = size,
    //             FileName = fileName,
    //             Post = new ElkPost
    //             {
    //                 Title = $"Title{i}",
    //                 Content = $"asd",
    //                 User = new ElkUser
    //                 {
    //                     FirstName = "User",
    //                     LastName = $"{i}",
    //                     DayOfBirth = DateTimeOffset.UtcNow,
    //                     Status = UserStatus.Active,
    //                     Address = $"NewYork{i}"
    //                 },
    //             },
    //         };

    //         medias.Add(media);
    //     }

    //     BulkResponse response = elasticClient.Bulk(b => b
    //         .Index(indexName)
    //         .CreateMany(medias)
    //         .Refresh(Refresh.WaitFor));

    //     if (response.IsValid)
    //     {
    //         Console.WriteLine("Elasticsearch has seeded.");
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Elasticsearch has been failed in seeding with {response.DebugInformation}");
    //     }
    // }
}

internal record ElsConfig(object Configs, Type Type);
