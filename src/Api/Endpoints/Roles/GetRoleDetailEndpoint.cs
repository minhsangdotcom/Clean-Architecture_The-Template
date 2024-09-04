using Application.UseCases.Roles.Queries.Detail;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class GetRoleDetailEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<string>.WithActionResult<ApiResponse>
{

    [HttpGet(Router.RoleRoute.GetUpdateDelete, Name = Router.RoleRoute.GetRouteName)]
    [SwaggerOperation(
            Tags = [Router.RoleRoute.Tags],
            Summary = "Get detail Role"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromRoute]string id, CancellationToken cancellationToken = default) =>
        new ApiResponse(await sender.Send(new GetRoleDetailQuery(Ulid.Parse(id)), cancellationToken), Message.SUCCESS);
}