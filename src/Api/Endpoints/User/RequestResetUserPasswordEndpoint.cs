using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Users.Commands.RequestResetPassword;
using Ardalis.ApiEndpoints;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class RequestResetUserPasswordEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<RequestResetUserPasswordCommand>.WithActionResult
{
    [HttpPut(Router.UserRoute.RequestResetPassowrd)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "request reset User password")]
    public override async Task<ActionResult> HandleAsync(
        RequestResetUserPasswordCommand request,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(request, cancellationToken);
        return this.NoContent204();
    }
}
