using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Roles.Queries.Detail;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class GetRoleDetailEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<string>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.RoleRoute.GetUpdateDelete, Name = Router.RoleRoute.GetRouteName)]
    [SwaggerOperation(Tags = [Router.RoleRoute.Tags], Summary = "Get detail Role")]
    [AuthorizeBy(permissions: $"{ActionPermission.detail}:{ObjectPermission.role}")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromRoute] string id,
        CancellationToken cancellationToken = default
    ) =>
        new ApiResponse(
            await sender.Send(new GetRoleDetailQuery(Ulid.Parse(id)), cancellationToken),
            Message.SUCCESS
        );
}
