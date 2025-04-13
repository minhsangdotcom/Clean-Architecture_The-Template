using Microsoft.AspNetCore.Http;

namespace Api.common.EndpointConfigurations;

public static class EndpointValidationExtension
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(
        this RouteHandlerBuilder route
    )
        where TRequest : class
    {
        return route
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json ");
    }
}
