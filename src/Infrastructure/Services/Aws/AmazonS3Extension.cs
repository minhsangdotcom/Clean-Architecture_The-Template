using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Aws;

public static class AmazonS3Extension
{
    public static IServiceCollection AddAmazonS3(this IServiceCollection services, IConfiguration configuration)
    {
        S3AwsSettings? s3AwsSettings = configuration.GetSection(nameof(S3AwsSettings)).Get<S3AwsSettings>();

        var clientConfig = new AmazonS3Config
        {
            ServiceURL = s3AwsSettings!.ServiceUrl,
            ForcePathStyle = true,
        };

        var s3Client = new AmazonS3Client(s3AwsSettings.AccessKey, s3AwsSettings.SecretKey, clientConfig);

        services.AddSingleton<IAmazonS3>(s3Client);
        services.Configure<S3AwsSettings>(options => configuration.GetSection(nameof(S3AwsSettings)).Bind(options));

        return services;
    }
}