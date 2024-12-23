using Application.Common.Auth;
using Application.Features.Users.Commands.Create;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class CreateUserEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<CreateUserCommand>.WithActionResult<ApiResponse>
{
    [HttpPost(Router.UserRoute.Users)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "create User")]
    [Restrict(claims: "permission:create.user")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromForm] CreateUserCommand request,
        CancellationToken cancellationToken = default
    )
    {
        CreateUserResponse user = await sender.Send(request, cancellationToken);
        return this.Created201(Router.UserRoute.GetRouteName, user.Id, user);
    }
}
