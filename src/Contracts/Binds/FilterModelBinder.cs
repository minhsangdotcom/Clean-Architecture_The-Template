using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.Binds;

public class FilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string[] queryString = GetQueryParams(bindingContext.HttpContext);
        bindingContext.Result = ModelBindingResult.Success(queryString);

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
