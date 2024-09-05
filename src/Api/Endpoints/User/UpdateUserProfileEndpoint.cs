using Application.UseCases.Users.Commands.Profiles;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class UpdateUserProfileEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<UpdateUserProfileQuery>.WithActionResult<ApiResponse>
{
    [HttpPut(Router.UserRoute.Profile)]
    [SwaggerOperation(
            Tags = [Router.UserRoute.Tags],
            Summary = "Update Profile User"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromForm] UpdateUserProfileQuery request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}