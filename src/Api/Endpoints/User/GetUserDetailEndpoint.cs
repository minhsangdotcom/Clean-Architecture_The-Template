using Application.UseCases.Users.Queries.Detail;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class GetUserDetailEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<string>.WithActionResult<ApiResponse>
{

    [HttpGet(Router.UserRoute.GetUpdateDelete, Name = Router.UserRoute.GetRouteName)]
    [SwaggerOperation(
            Tags = [Router.UserRoute.Tags],
            Summary = "Detail User"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromRoute(Name = Router.Id)] string userId, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(new GetUserDetailQuery(Ulid.Parse(userId)), cancellationToken));
}