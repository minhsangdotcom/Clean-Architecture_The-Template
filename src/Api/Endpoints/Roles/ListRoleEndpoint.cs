using Api.common.Documents;
using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Roles.Queries.List;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Roles;

public class ListRoleEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.RoleRoute.Roles, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get list of roles",
                Description = "Returns a list of roles",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
                Parameters = operation.AddDocs(),
            });
    }

    private async Task<
        Results<Ok<ApiResponse<IEnumerable<ListRoleResponse>>>, ProblemHttpResult>
    > HandleAsync(
        ListRoleQuery request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
