namespace Api.Extensions;

public static class EnvironmentConfigurationExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        string environmentName = builder.Environment.EnvironmentName;
        builder
            .Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFiles(environmentName, "appsettings")
            .AddEnvironmentVariables();
    }

    private static IConfigurationBuilder AddJsonFiles(
        this IConfigurationBuilder builder,
        string environmentName = "Development",
        string fileName = "appsettings"
    )
    {
        return builder.AddJsonFile(
            $"{fileName}.{environmentName}.json",
            optional: true,
            reloadOnChange: true
        );
    }
}
