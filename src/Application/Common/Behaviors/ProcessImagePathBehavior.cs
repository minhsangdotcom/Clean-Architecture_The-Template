using System.Collections;
using System.Reflection;
using Application.Common.Interfaces.Services.Storage;
using Application.Common.Security;
using Mediator;
using Serilog;
using SharedKernel.Responses;

namespace Application.Common.Behaviors;

public class ProcessImagePathBehavior<TMessage, TResponse>(
    ILogger logger,
    IStorageService storageService
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
    private static bool IsPaginationResponse() =>
        typeof(TResponse).IsGenericType
        && typeof(TResponse).GetGenericTypeDefinition() == typeof(PaginationResponse<>);

    // Processes responses of type PaginationResponse<>
    private void ProcessPaginationResponse(TResponse response)
    {
        PropertyInfo? dataProperty = GetProperty(nameof(PaginationResponse<object>.Data));
        if (dataProperty == null)
            return;

        object? dataPropertyValue = dataProperty.GetValue(response);
        if (dataPropertyValue is IEnumerable dataEnumerable)
        {
            foreach (object data in dataEnumerable)
            {
                ProcessDataPropertiesWithFileAttribute(data);
            }
        }
    }

    // Processes individual response properties with the [File] attribute
    private void ProcessSingleResponse(TResponse response)
    {
        PropertyInfo? property = GetFileAttributeProperty(typeof(TResponse));
        if (property == null)
            return;

        object? key = property.GetValue(response);
        if (key == null)
            return;

        UpdatePropertyIfNotPublicUrl(response!, property, key);
    }

    // Processes the properties of a data object within a pagination response
    private void ProcessDataPropertiesWithFileAttribute(object data)
    {
        IEnumerable<PropertyInfo> propertiesWithFileAttribute = GetFileAttributeProperties(
            data.GetType()
        );

        foreach (PropertyInfo prop in propertiesWithFileAttribute)
        {
            object? imageKey = prop.GetValue(data);
            if (imageKey == null)
                continue;

            logger.Information("image key {value}", imageKey);

            UpdatePropertyIfNotPublicUrl(data, prop, imageKey);
        }
    }

    // Updates the property value if the key does not already have http url
    private void UpdatePropertyIfNotPublicUrl(object target, PropertyInfo property, object key)
    {
        string imageKeyStr = key.ToString()!;
        if (!imageKeyStr.StartsWith(storageService.GetPublicUrl()!))
        {
            string? fullPath = storageService.GetFullpath(imageKeyStr);
            logger.Information("image path {value}", fullPath);
            property.SetValue(target, fullPath);
        }
    }

    // Retrieves a property by name from the TResponse type
    private static PropertyInfo? GetProperty(string propertyName) =>
        typeof(TResponse).GetProperty(propertyName);

    // Retrieves the first property with a [File] attribute from the given type
    private static PropertyInfo? GetFileAttributeProperty(Type type) =>
        type.GetProperties()
            .FirstOrDefault(prop =>
                prop.CustomAttributes.Any(attr =>
                    attr.AttributeType.FullName == typeof(FileAttribute).FullName
                )
            );

    // Retrieves all properties with the [File] attribute from the given type
    private static IEnumerable<PropertyInfo> GetFileAttributeProperties(Type type) =>
        type.GetProperties()
            .Where(prop =>
                prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(FileAttribute))
            );
}
