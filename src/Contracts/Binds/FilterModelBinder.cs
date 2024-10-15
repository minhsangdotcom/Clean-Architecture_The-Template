using Contracts.Dtos.Requests;
using Contracts.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.Binds;

public class FilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var queryString = GetQueryParams(bindingContext.HttpContext);

        object? filters = StringExtension.Parse(queryString);

        if (filters != null)
        {
            Console.WriteLine(
                "🚀 ~ filters have been successfully bound:"
                    + SerializerExtension.Serialize(filters!)
            );
        }

        bindingContext.Result = ModelBindingResult.Success(filters);

        return Task.CompletedTask;
    }

    private static string[] GetQueryParams(HttpContext httpContext)
    {
        var queryStringValue = httpContext?.Request.QueryString.Value;

        if (string.IsNullOrEmpty(queryStringValue))
            return [];

        var queryParams = queryStringValue[1..].Split("&");

        return queryParams
            .Where(param =>
                param.StartsWith(
                    nameof(QueryParamRequest.Filter),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            .ToArray();
    }
}
