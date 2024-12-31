using Application.Features.Users.Commands.Token;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class RefreshUserTokenEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<RefreshUserTokenCommand>.WithActionResult<ApiResponse>
{
    private readonly ISender sender = sender;

    [HttpPost(Router.UserRoute.RefreshToken)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "refresh token")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        RefreshUserTokenCommand request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
