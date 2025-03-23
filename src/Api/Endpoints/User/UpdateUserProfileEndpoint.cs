using Api.common.RouteResults;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Users.Commands.Profiles;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class UpdateUserProfileEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<UpdateUserProfileCommand>.WithActionResult<
        ApiResponse<UpdateUserProfileResponse>
    >
{
    [HttpPut(Router.UserRoute.Profile)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Update Profile User")]
    [AuthorizeBy]
    public override async Task<ActionResult<ApiResponse<UpdateUserProfileResponse>>> HandleAsync(
        [FromForm] UpdateUserProfileCommand request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
