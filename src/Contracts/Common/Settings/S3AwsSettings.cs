namespace Contracts.Common.Settings;

public class S3AwsSettings
{
    public string? ServiceUrl { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    public string? BucketName { get; set; }

    public string? PublicUrl { get; set; }

    public string? PreSignedUrlExpirationInMinutes { get; set; }
}