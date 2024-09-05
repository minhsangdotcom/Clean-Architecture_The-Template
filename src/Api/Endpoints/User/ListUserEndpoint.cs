using Application.UseCases.Users.Queries.List;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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