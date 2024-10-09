using Contracts.Dtos.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.Binds;

public class FilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var query = bindingContext
            .HttpContext.Request.QueryString.Value![1..]
            .Split("&")
            .Where(x =>
                x.StartsWith(nameof(QueryParamRequest.Filter), StringComparison.OrdinalIgnoreCase)
            )
            .ToArray();

        bindingContext.Result = ModelBindingResult.Success("");

        return Task.CompletedTask;
    }
}
