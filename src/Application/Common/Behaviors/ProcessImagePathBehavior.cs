using System.Reflection;
using Application.Common.Interfaces.Services;
using Application.Common.Security;
using Contracts.Common.Settings;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Common.Behaviors;

public class ProcessImagePathBehavior<TMessage, TResponse>(
    ILogger<TResponse> logger,
    IAwsAmazonService awsAmazonService,
    IOptions<S3AwsSettings> options
    ) : MessagePostProcessor<TMessage, TResponse> where TMessage : notnull, IMessage
{
    private readonly S3AwsSettings s3AwsSettings = options.Value;

    protected override ValueTask Handle(TMessage message, TResponse response, CancellationToken cancellationToken)
    {
        PropertyInfo? property = typeof(TResponse).GetProperties()
            .FirstOrDefault(
                prop =>
                    prop.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(FileAttribute).FullName)
            );

        if (property == null)
        {
            return default!;
        }

        object? key = property.GetValue(response);

        logger.LogInformation("image key {value}", key);

        if (key == null)
        {
            return default!;
        }

        if (key.ToString()!.StartsWith(s3AwsSettings.PublicUrl!))
        {
            return default!;
        }

        string? path = awsAmazonService.GetFullpath(key?.ToString());

        logger.LogInformation("image path {value}", path);

        property.SetValue(response, path);
        
        return default!;
    }
}
