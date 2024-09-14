using Application.Common.Auth;
using Application.UseCases.Users.Commands.ChangePassword;
using Ardalis.ApiEndpoints;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class ChangeUserPasswordEnpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ChangeUserPasswordCommand>.WithActionResult
{
    [HttpPut(Router.UserRoute.ChangePassword)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Change User password")]
    [Restrict()]
    public override async Task<ActionResult> HandleAsync(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(request, cancellationToken);
        return this.NoContent204();
    }
}
