using System.Collections;
using System.Reflection;
using Application.Common.Interfaces.Services.Aws;
using Application.Common.Security;
using Contracts.Dtos.Responses;
using Mediator;
using Serilog;

namespace Application.Common.Behaviors;

public class ProcessImagePathBehavior<TMessage, TResponse>(
    ILogger logger,
    IAwsAmazonService awsAmazonService
) : MessagePostProcessor<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask Handle(
        TMessage message,
        TResponse response,
        CancellationToken cancellationToken
    )
    {
        if (
            typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(PaginationResponse<>)
        )
        {
            PropertyInfo? dataProperty = typeof(TResponse).GetProperty(
                nameof(PaginationResponse<object>.Data)
            );
            object? dataPropertyValue = dataProperty?.GetValue(response);

            if (dataPropertyValue is IEnumerable dataEnumerable)
            {
                foreach (var data in dataEnumerable)
                {
                    var propertiesWithFileAttribute = data.GetType()
                        .GetProperties()
                        .Where(prop =>
                            prop.CustomAttributes.Any(attr =>
                                attr.AttributeType == typeof(FileAttribute)
                            )
                        );

                    foreach (var prop in propertiesWithFileAttribute)
                    {
                        object? imageKey = prop.GetValue(data);

                        if (imageKey == null)
                        {
                            continue;
                        }

                        logger.Information("image key {value}", imageKey);

                        string imageKeyStr = imageKey.ToString()!;
                        if (!imageKeyStr.StartsWith(awsAmazonService.GetPublicUrl()!))
                        {
                            string? fullPath = awsAmazonService.GetFullpath(imageKeyStr);
                            logger.Information("image path {value}", fullPath);
                            prop.SetValue(data, fullPath);
                        }
                    }
                }
            }

            return default!;
        }

        PropertyInfo? property = typeof(TResponse)
            .GetProperties()
            .FirstOrDefault(prop =>
                prop.CustomAttributes.Any(attr =>
                    attr.AttributeType.FullName == typeof(FileAttribute).FullName
                )
            );

        if (property == null)
        {
            return default!;
        }

        object? key = property.GetValue(response);

        logger.Information("image key {value}", key);

        if (key == null)
        {
            return default!;
        }

        if (key.ToString()!.StartsWith(awsAmazonService.GetPublicUrl()!))
        {
            return default!;
        }

        string? path = awsAmazonService.GetFullpath(key?.ToString());

        logger.Information("image path {value}", path);

        property.SetValue(response, path);

        return default!;
    }
}
