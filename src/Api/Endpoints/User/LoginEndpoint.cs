using Application.Features.Users.Commands.Login;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class LoginEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<LoginUserCommand>.WithActionResult<ApiResponse>
{
    [HttpPost(Router.UserRoute.Login)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Logging in User")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        LoginUserCommand request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
