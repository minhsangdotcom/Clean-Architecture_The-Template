using Contracts.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.Binds;

public class FilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string[] queryString = GetQueryParams(bindingContext.HttpContext);
        object? filters = StringExtension.Parse(queryString);

        if (filters != null)
        {
            Console.WriteLine(
                "filters have been successfully bound:"
                    + SerializerExtension.Serialize(filters!).StringJson
            );
        }

        bindingContext.Result = ModelBindingResult.Success(filters);

        return Task.CompletedTask;
    }

    private static string[] GetQueryParams(HttpContext httpContext)
    {
        string? queryStringValue = httpContext?.Request.QueryString.Value;

        if (string.IsNullOrEmpty(queryStringValue))
        {
            return [];
        }

        return ModelBindingExtension.GetFilterQueries(queryStringValue);
    }
}
