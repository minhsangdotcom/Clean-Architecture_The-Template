using System.Reflection;
using Domain.Aggregates.AuditLogs;
using Domain.Aggregates.AuditLogs.Enums;
using Domain.Aggregates.Users.Enums;
using Elastic.Clients.Elasticsearch;
using Serilog;
using SharedKernel.Common.ElasticConfigurations;

namespace Infrastructure.Services.Elastics;

public class ElasticsearchRegisterHelper
{
    /// <summary>
    /// Execute connection mapping config
    /// </summary>
    /// <param name="connectionSettings"></param>
    /// <param name="elsConfigs"></param>
    public static void ConfigureConnectionSettings(
        ref ElasticsearchClientSettings connectionSettings,
        IEnumerable<ElasticConfigureResult> configures
    )
    {
        foreach (var configure in configures)
        {
            object? connectionSettingEvaluator = Activator.CreateInstance(
                typeof(ConnectionSettingEvaluator),
                [connectionSettings]
            );

            var evaluateMethodInfo = typeof(ConnectionSettingEvaluator)
                .GetMethod(nameof(IEvaluatorSync.Evaluate))!
                .MakeGenericMethod(configure.Type);

            evaluateMethodInfo.Invoke(connectionSettingEvaluator, [configure.Configs]);
        }
    }

    /// <summary>
    /// execute config classes by reflection
    /// </summary>
    /// <param name="elasticClient"></param>
    /// <param name="elsConfigs"></param>
    /// <returns></returns>
    public static async Task ElasticFluentConfigAsync(
        ElasticsearchClient elasticClient,
        IEnumerable<ElasticConfigureResult> configures
    )
    {
        foreach (var configure in configures)
        {
            object? elasticsearchClientEvaluator = Activator.CreateInstance(
                typeof(ElasticsearchClientEvaluator),
                [elasticClient]
            );

            var evaluateMethodInfo = typeof(ElasticsearchClientEvaluator)
                .GetMethod(nameof(IEvaluator.Evaluate))!
                .MakeGenericMethod(configure.Type);

            await (Task)
                evaluateMethodInfo.Invoke(elasticsearchClientEvaluator, [configure.Configs])!;
        }
    }

    /// <summary>
    /// fake data for test
    /// </summary>
    /// <param name="elasticsearchClient"></param>
    /// <returns></returns>
    public static async Task SeedingAsync(ElasticsearchClient elasticsearchClient)
    {
        var auditLog = await elasticsearchClient.SearchAsync<AuditLog>();
        if (auditLog.Documents.Count > 0)
        {
            return;
        }

        List<AuditLog> auditLogs =
        [
            new AuditLog()
            {
                Id = "01J8HF5FF082A9EPSDYEF02ENY",
                Entity =
                    "Theo Heatworld, Victoria Beckham đang rất lo lắng khi David Beckham dồn hết tâm huyết và thời gian cho điền trang Cotswolds. Như Heatworld đưa tin trước đó, cặp đôi này đã cân nhắc chuyển đến sống ở khu điền trang trị giá 12 triệu bảng Anh của họ ở Oxfordshire suốt một thời gian dài. David gần đây chia sẻ trên Instagram khung cảnh của điền trang cũng như việc làm khoai tây chiên giòn do anh tự trồng và thu thập trứng gà.",
                Type = (int)AuditLogType.Create,
                ActionPerformBy = "01J8HF8PE8SYB0NRVCC8CZGA11",
                Agent = new Agent()
                {
                    Id = "01J8HF8PE8SYB0NRVCC8CZGA11",
                    CreatedAt = DateTimeOffset.Parse("2024-09-24T07:35:30Z"),
                    FirstName = "Sáng",
                    LastName = "Trần Minh Sáng",
                    Email = "anna.kim71@gmail.com",
                    DayOfBirth = DateTime.Parse("1990-07-08T00:00:00"),
                    Gender = (int)Gender.Other,
                    Role = new RoleTest()
                    {
                        Name = "ADMIN",
                        Guard =
                            "Nhiễm khuẩn cũng có thể xảy ra sau khi trứng được đẻ. Khi đó, con gà nhiễm Salmonella trong ruột và thải ra qua phân, vi khuẩn có thể bám ra bên ngoài vỏ trứng trong quá trình làm tổ. Do đó, để giảm nguy cơ nhiễm Salmonella từ vỏ trứng, Bộ Nông nghiệp Mỹ yêu cầu trứng phải được rửa sạch trước khi bán.",
                        Permissions =
                        [
                            new PermissionTest() { Name = "update" },
                            new PermissionTest() { Name = "create" },
                        ],
                    },
                },
                CreatedAt = DateTimeOffset.Parse("2024-09-24T07:35:30Z"),
            },
            new AuditLog()
            {
                Id = "01J8HGCH0REE7YE3K2997XB00X",
                Entity =
                    "Trong buổi phỏng vấn vào tháng 7, Karim Benzema, đồng đội cũ của Vinicius tại Real Madrid, cũng dự đoán chiến thắng thuộc về chân sút sinh năm 2000. 'Chủ nhân Quả bóng vàng sao? Tôi sẽ chọn Vinicius, đơn giản là vì cậu ấy xứng đáng. Không chỉ năm nay, mà năm ngoái cậu ấy cũng chơi rất hay. Vini vượt trội so với các cầu thủ khác ở kỹ năng cá nhân. Cậu ấy đã là một cầu thủ rất toàn diện'.",
                Type = (int)AuditLogType.Create,
                ActionPerformBy = "01J8HF8PE8SYB0NRVCC8CZGA11",
                Agent = new Agent()
                {
                    Id = "01J8HF8PE8SYB0NRVCC8CZGA11",
                    CreatedAt = DateTimeOffset.Parse("2024-09-24T07:55:26Z"),
                    FirstName = "Tiên",
                    LastName = "Nguyễn",
                    Email = "anna.kim71@gmail.com",
                    DayOfBirth = DateTime.Parse("1990-07-09T00:00:00"),
                    Gender = (int)Gender.Female,
                    Role = new RoleTest()
                    {
                        Name = "MANAGER",
                        Guard =
                            "Đến giữa tháng 7/2022, VinFast tuyên bố ngừng kinh doanh xe xăng để trở thành một hãng xe thuần điện. Khi ấy, khách hàng trong nước đã quen với VF e34, được VinFast bắt đầu bàn giao từ tháng 12/2021.",
                        Permissions =
                        [
                            new PermissionTest() { Name = "delete" },
                            new PermissionTest() { Name = "list" },
                        ],
                    },
                },
                CreatedAt = DateTimeOffset.Parse("2024-09-24T07:55:26Z"),
            },
            new AuditLog()
            {
                Id = "01J8HGNPN8B8HT3GFBAFYPMENK",
                Entity =
                    "Trong số nhiều trang bị bôi đen, tài liệu pháp lý bao gồm những cáo buộc cho rằng các thí sinh phải chịu đựng môi trường làm việc 'dung dưỡng văn hóa kỳ thị giới và phân biệt đối xử với phụ nữ'. Những cáo buộc này đánh vỡ hình ảnh của MrBeast, thường được xem là một trong những người tử tế nhất trên Internet.",
                Type = (int)AuditLogType.Update,
                ActionPerformBy = "01J8HHP6VGVDGWSP08KG82AFCD",
                Agent = new Agent()
                {
                    Id = "01J8HHP6VGVDGWSP08KG82AFCD",
                    CreatedAt = DateTimeOffset.Parse("2024-09-24T08:17:26Z"),
                    FirstName = "Hiếu",
                    LastName = "Trần Minh Hiếu",
                    Email = "anna.kim71@gmail.com",
                    DayOfBirth = DateTime.Parse("1990-07-10T00:00:00"),
                    Gender = (int)Gender.Male,
                    Role = new RoleTest()
                    {
                        Name = "USER",
                        Guard =
                            "Càng đi sâu ở Anh trai vượt ngàn chông gai, Soobin Hoàng Sơn càng phát huy thế mạnh của một ca sĩ toàn năng. Tại công diễn 4, thành viên của SpaceSpeakers ghi dấu ấn ở các vai trò hát, rap và trình diễn sân khấu. Việc dẫn đầu bảng điểm hỏa lực cá nhân với 1.130, cách biệt với các đối thủ còn lại, là lời hồi đáp của Soobin sau một hành trình dài tại chương trình.",
                        Permissions = [new PermissionTest() { Name = "list" }],
                    },
                },
                CreatedAt = DateTimeOffset.Parse("2024-09-24T08:17:26Z"),
            },
        ];

        var response = await elasticsearchClient.IndexManyAsync(
            auditLogs,
            ElsIndexExtension.GetName<AuditLog>()
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

    /// <summary>
    /// get all of config classes by reflection
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<ElasticConfigureResult> GetElasticsearchConfigBuilder(
        Assembly assembly
    )
    {
        var configuringTypes = GetConfiguringTypes(assembly);

        foreach (var (type, iType) in configuringTypes)
        {
            var method = GetConfigureMethod(type);
            if (method == null)
                continue;

            var elasticsearchConfigBuilder = CreateElasticsearchConfigBuilder(iType);
            var elsConfig = Activator.CreateInstance(type);

            method.Invoke(elsConfig, [elasticsearchConfigBuilder]);

            yield return new ElasticConfigureResult(elasticsearchConfigBuilder!, iType);
        }
    }

    private static IEnumerable<(Type type, Type iType)> GetConfiguringTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type =>
                type.GetInterfaces().Any(@interface => IsElasticsearchDocumentConfigure(@interface))
            )
            .Select(type =>
                (
                    type,
                    iType: type.GetInterfaces()
                        .First(@interface => IsElasticsearchDocumentConfigure(@interface))
                        .GenericTypeArguments[0]
                )
            );
    }

    private static bool IsElasticsearchDocumentConfigure(Type @interface)
    {
        return @interface.IsGenericType
            && @interface.GetGenericTypeDefinition() == typeof(IElasticsearchDocumentConfigure<>);
    }

    private static MethodInfo? GetConfigureMethod(Type type)
    {
        return type.GetMethod(nameof(IElasticsearchDocumentConfigure<object>.Configure));
    }

    private static object CreateElasticsearchConfigBuilder(Type documentType)
    {
        var builderType = typeof(ElasticsearchConfigBuilder<>).MakeGenericType(documentType);
        return Activator.CreateInstance(builderType)!;
    }

    private static IEnumerable<PermissionTest> Generate(int quantity, List<string> permissions)
    {
        var ran = new Random();
        for (int i = 0; i < quantity; i++)
        {
            yield return new PermissionTest()
            {
                Name = permissions[ran.Next(0, permissions.Count - 1)],
            };
        }
    }
}
