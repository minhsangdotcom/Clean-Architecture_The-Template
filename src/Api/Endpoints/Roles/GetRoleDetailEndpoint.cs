using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Roles.Queries.Detail;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Roles;

public class GetRoleDetailEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.RoleRoute.GetUpdateDelete, HandleAsync)
            .WithName(Router.RoleRoute.GetRouteName)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get detail of existed Role",
                Description = "Returns the role if found",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
            });
    }

    private async Task<Results<Ok<ApiResponse<RoleDetailResponse>>, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new GetRoleDetailQuery(Ulid.Parse(id));
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}
