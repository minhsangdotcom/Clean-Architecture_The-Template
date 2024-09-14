using Application.UseCases.Users.Queries.Profiles;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.RouteResults;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class GetUserProfileEndpoint(ISender sender) : EndpointBaseAsync.WithoutRequest.WithActionResult<ApiResponse>
{
    [HttpGet(Router.UserRoute.Profile)]
    [SwaggerOperation(
            Tags = [Router.UserRoute.Tags],
            Summary = "Profile User"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(new GetUserProfileQuery(), cancellationToken));
}