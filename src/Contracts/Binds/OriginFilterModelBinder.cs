using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.Binds;

public class OriginFilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string? query = bindingContext.HttpContext?.Request.QueryString.Value;

        bindingContext.Result = ModelBindingResult.Success(query);

        return Task.CompletedTask;
    }
}
