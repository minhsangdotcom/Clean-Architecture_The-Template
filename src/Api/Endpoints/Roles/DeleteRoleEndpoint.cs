using Application.UseCases.Roles.Commands.Delete;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.RouteResults;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class DeleteRoleEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<string>.WithActionResult<ApiResponse>
{
    [HttpDelete(Router.RoleRoute.GetUpdateDelete)]
    [SwaggerOperation(
            Tags = [Router.RoleRoute.Tags],
            Summary = "Delete Role"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromRoute(Name = Router.Id)] string roleId, CancellationToken cancellationToken = default)
    {
        await sender.Send(new DeleteRoleCommand(Ulid.Parse(roleId)), cancellationToken);
        return this.NoContent204();
    }
}