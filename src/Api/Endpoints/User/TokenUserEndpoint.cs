using Application.UseCases.Users.Commands.Token;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.RouteResults;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class TokenUserEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<UserTokenCommand>.WithActionResult<ApiResponse>
{
    private readonly ISender sender = sender;

    [HttpPost(Router.LoginRoute.Token)]
    [SwaggerOperation(
            Tags = [Router.LoginRoute.Tags],
            Summary = "refresh token"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(UserTokenCommand request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}