using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.RouteResults;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Application.Features.Users.Queries.List;

namespace Api.Endpoints.User;

public class ListUserEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<ListUserQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.UserRoute.Users)]
    [SwaggerOperation(
           Tags = [Router.UserRoute.Tags],
           Summary = "list User"
       )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromQuery] ListUserQuery request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}