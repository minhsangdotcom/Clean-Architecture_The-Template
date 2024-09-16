using Application.UseCases.Users.Commands.RequestForgotPassword;
using Ardalis.ApiEndpoints;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class RequestForgotUserPasswordEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<RequestForgotUserPasswordCommand>.WithActionResult
{
    [HttpPut(Router.UserRoute.RequestResetPassowrd)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Change User password")]
    public override async Task<ActionResult> HandleAsync(
        RequestForgotUserPasswordCommand request,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(request,cancellationToken);
        return this.NoContent204();
    }
}
