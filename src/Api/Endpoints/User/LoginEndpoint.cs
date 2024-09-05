using Application.UseCases.Users.Commands.Login;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class LoginEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<LoginUserCommand>.WithActionResult<ApiResponse>
{

    [HttpPost(Router.AuthRoute.Auths)]
    [SwaggerOperation(
            Tags = [Router.AuthRoute.Tags],
            Summary = "Auth User"
        )]
    public async override Task<ActionResult<ApiResponse>> HandleAsync(LoginUserCommand request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}