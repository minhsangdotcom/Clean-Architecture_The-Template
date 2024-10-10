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

        bindingContext.Result = ModelBindingResult.Success(StringExtension.Parse(queryString));

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
