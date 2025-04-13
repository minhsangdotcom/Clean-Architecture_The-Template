using Api.common.Documents;
using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Permissions;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Permissions;

public class ListPermissionEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.PermissionRoute.Permissions, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get Permissions in system",
                Description = "Returns a list of permissions in the system.",
                Tags = [new OpenApiTag() { Name = Router.PermissionRoute.Tags }],
                Parameters = operation.AddDocs(),
            });
    }

    private async Task<
        Results<Ok<ApiResponse<IEnumerable<ListPermissionResponse>>>, ProblemHttpResult>
    > HandleAsync(
        ListPermissionQuery request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
