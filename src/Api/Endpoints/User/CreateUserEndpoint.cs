// using Api.common.RouteResults;
// using Api.common.Routers;
// using Application.Common.Auth;
// using Application.Features.Users.Commands.Create;
// using Ardalis.ApiEndpoints;
// using Contracts.ApiWrapper;
// using Infrastructure.Constants;
// using Mediator;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;

using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Users.Commands.Create;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class CreateUserEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.UserRoute.Users, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create user",
                Description = "Returns a new user",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<CreateUserCommand>();
    }

    private async Task<
        Results<CreatedAtRoute<ApiResponse<CreateUserResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromForm] CreateUserCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToCreatedResult(result.Value!.Id, Router.UserRoute.GetRouteName);
    }
}
