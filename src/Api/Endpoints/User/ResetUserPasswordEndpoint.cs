using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Users.Commands.ResetPassword;
using Ardalis.ApiEndpoints;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class ResetUserPasswordEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ResetUserPasswordCommand>.WithActionResult
{
    [HttpPut(Router.UserRoute.ResetPassowrd)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "reset User password")]
    public override async Task<ActionResult> HandleAsync(
        ResetUserPasswordCommand request,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(request, cancellationToken);
        return this.NoContent204();
    }
}
