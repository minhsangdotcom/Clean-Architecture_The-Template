using Api.common.RouteResults;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Users.Commands.Delete;
using Ardalis.ApiEndpoints;
using Contracts.Constants;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class DeleteUserEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<string>.WithActionResult
{
    [HttpDelete(Router.UserRoute.GetUpdateDelete)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Delete User")]
    [AuthorizeBy(permissions: $"{ActionPermission.delete}:{ObjectPermission.user}")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute(Name = RoutePath.Id)] string userId,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new DeleteUserCommand(Ulid.Parse(userId)), cancellationToken);
        return this.NoContent204();
    }
}
