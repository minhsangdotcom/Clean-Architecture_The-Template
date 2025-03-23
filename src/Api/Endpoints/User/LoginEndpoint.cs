using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Users.Commands.Login;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class LoginEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<LoginUserCommand>.WithActionResult<
        ApiResponse<LoginUserResponse>
    >
{
    [HttpPost(Router.UserRoute.Login)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Logging in User")]
    public override async Task<ActionResult<ApiResponse<LoginUserResponse>>> HandleAsync(
        LoginUserCommand request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
