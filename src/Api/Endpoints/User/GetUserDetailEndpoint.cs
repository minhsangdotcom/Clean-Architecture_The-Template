using Application.Common.Auth;
using Application.Features.Users.Queries.Detail;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class GetUserDetailEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<string>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.UserRoute.GetUpdateDelete, Name = Router.UserRoute.GetRouteName)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "Detail User")]
    [AuthorizeBy(
        permissions: $"{Credential.ActionPermission.create}:{Credential.ObjectPermission.user}"
    )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromRoute(Name = Router.Id)] string userId,
        CancellationToken cancellationToken = default
    ) =>
        this.Ok200(
            await sender.Send(new GetUserDetailQuery(Ulid.Parse(userId)), cancellationToken)
        );
}
