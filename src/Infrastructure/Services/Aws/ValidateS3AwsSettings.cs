using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Aws;

public class ValidateS3AwsSettings : IValidateOptions<S3AwsSettings>
{
    public ValidateOptionsResult Validate(string? name, S3AwsSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.AccessKey))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.AccessKey)} must not be null or empty"
            );
        }

        if (string.IsNullOrWhiteSpace(options.BucketName))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.BucketName)} must not be null or empty"
            );
        }

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.SecretKey)} must not be null or empty"
            );
        }

        if (string.IsNullOrWhiteSpace(options.PublicUrl))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.PublicUrl)} must not be null or empty"
            );
        }

        if (string.IsNullOrWhiteSpace(options.ServiceUrl))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.ServiceUrl)} must not be null or empty"
            );
        }

        if (string.IsNullOrWhiteSpace(options.PreSignedUrlExpirationInMinutes))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.PreSignedUrlExpirationInMinutes)} must not be null or empty"
            );
        }

        return ValidateOptionsResult.Success;
    }
}
