using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Roles.Commands.Create;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Roles;

public class CreateRoleEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.RoleRoute.Roles, HandleAsync)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Create Role",
                Description = "Returns a new role",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
            })
            .WithRequestValidation<CreateRoleCommand>();
    }

    private async Task<
        Results<CreatedAtRoute<ApiResponse<CreateRoleResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromBody] CreateRoleCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToCreatedResult(result.Value!.Id);
    }
}
