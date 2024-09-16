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
        // Check if the response is a PaginationResponse and handle accordingly
        if (ProcessImagePathBehavior<TMessage, TResponse>.IsPaginationResponse())
        {
            ProcessPaginationResponse(response);
            return default!;
        }

        // Handle non-pagination responses
        ProcessSingleResponse(response);

        return default!;
    }

    // Checks if TResponse is of PaginationResponse<> type
    private static bool IsPaginationResponse()
    {
        return typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(PaginationResponse<>);
    }

    // Processes responses of type PaginationResponse<>
    private void ProcessPaginationResponse(TResponse response)
    {
        var dataProperty = ProcessImagePathBehavior<TMessage, TResponse>.GetProperty(nameof(PaginationResponse<object>.Data));
        if (dataProperty == null)
            return;

        var dataPropertyValue = dataProperty.GetValue(response);
        if (dataPropertyValue is IEnumerable dataEnumerable)
        {
            foreach (var data in dataEnumerable)
            {
                ProcessDataPropertiesWithFileAttribute(data);
            }
        }
    }

    // Processes individual response properties with the [File] attribute
    private void ProcessSingleResponse(TResponse response)
    {
        var property = ProcessImagePathBehavior<TMessage, TResponse>.GetFileAttributeProperty(typeof(TResponse));
        if (property == null)
            return;

        var key = property.GetValue(response);
        if (key == null)
            return;

        UpdatePropertyIfNotPublicUrl(response!, property, key);
    }

    // Processes the properties of a data object within a pagination response
    private void ProcessDataPropertiesWithFileAttribute(object data)
    {
        var propertiesWithFileAttribute = ProcessImagePathBehavior<TMessage, TResponse>.GetFileAttributeProperties(data.GetType());

        foreach (var prop in propertiesWithFileAttribute)
        {
            var imageKey = prop.GetValue(data);
            if (imageKey == null)
                continue;

            logger.Information("image key {value}", imageKey);

            UpdatePropertyIfNotPublicUrl(data, prop, imageKey);
        }
    }

    // Updates the property value if the key does not already have a public URL
    private void UpdatePropertyIfNotPublicUrl(object target, PropertyInfo property, object key)
    {
        string imageKeyStr = key.ToString()!;
        if (!imageKeyStr.StartsWith(awsAmazonService.GetPublicUrl()!))
        {
            var fullPath = awsAmazonService.GetFullpath(imageKeyStr);
            logger.Information("image path {value}", fullPath);
            property.SetValue(target, fullPath);
        }
    }

    // Retrieves a property by name from the TResponse type
    private static PropertyInfo? GetProperty(string propertyName)
    {
        return typeof(TResponse).GetProperty(propertyName);
    }

    // Retrieves the first property with a [File] attribute from the given type
    private static PropertyInfo? GetFileAttributeProperty(Type type)
    {
        return type.GetProperties()
            .FirstOrDefault(prop =>
                prop.CustomAttributes.Any(attr =>
                    attr.AttributeType.FullName == typeof(FileAttribute).FullName
                )
            );
    }

    // Retrieves all properties with the [File] attribute from the given type
    private static IEnumerable<PropertyInfo> GetFileAttributeProperties(Type type)
    {
        return type.GetProperties()
            .Where(prop =>
                prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(FileAttribute))
            );
    }

    // protected override ValueTask Handle(
    //     TMessage message,
    //     TResponse response,
    //     CancellationToken cancellationToken
    // )
    // {
    //     if (
    //         typeof(TResponse).IsGenericType
    //         && typeof(TResponse).GetGenericTypeDefinition() == typeof(PaginationResponse<>)
    //     )
    //     {
    //         PropertyInfo? dataProperty = typeof(TResponse).GetProperty(
    //             nameof(PaginationResponse<object>.Data)
    //         );
    //         object? dataPropertyValue = dataProperty?.GetValue(response);

    //         if (dataPropertyValue is IEnumerable dataEnumerable)
    //         {
    //             foreach (var data in dataEnumerable)
    //             {
    //                 var propertiesWithFileAttribute = data.GetType()
    //                     .GetProperties()
    //                     .Where(prop =>
    //                         prop.CustomAttributes.Any(attr =>
    //                             attr.AttributeType == typeof(FileAttribute)
    //                         )
    //                     );

    //                 foreach (var prop in propertiesWithFileAttribute)
    //                 {
    //                     object? imageKey = prop.GetValue(data);

    //                     if (imageKey == null)
    //                     {
    //                         continue;
    //                     }

    //                     logger.Information("image key {value}", imageKey);

    //                     string imageKeyStr = imageKey.ToString()!;
    //                     if (!imageKeyStr.StartsWith(awsAmazonService.GetPublicUrl()!))
    //                     {
    //                         string? fullPath = awsAmazonService.GetFullpath(imageKeyStr);
    //                         logger.Information("image path {value}", fullPath);
    //                         prop.SetValue(data, fullPath);
    //                     }
    //                 }
    //             }
    //         }

    //         return default!;
    //     }

    //     PropertyInfo? property = typeof(TResponse)
    //         .GetProperties()
    //         .FirstOrDefault(prop =>
    //             prop.CustomAttributes.Any(attr =>
    //                 attr.AttributeType.FullName == typeof(FileAttribute).FullName
    //             )
    //         );

    //     if (property == null)
    //     {
    //         return default!;
    //     }

    //     object? key = property.GetValue(response);

    //     logger.Information("image key {value}", key);

    //     if (key == null)
    //     {
    //         return default!;
    //     }

    //     if (key.ToString()!.StartsWith(awsAmazonService.GetPublicUrl()!))
    //     {
    //         return default!;
    //     }

    //     string? path = awsAmazonService.GetFullpath(key?.ToString());

    //     logger.Information("image path {value}", path);

    //     property.SetValue(response, path);

    //     return default!;
    // }
}
