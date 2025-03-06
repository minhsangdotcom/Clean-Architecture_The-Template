using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.AuditLogs.Queries;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.AuditLogs;

public class ListAuditLog(ISender sender)
    : EndpointBaseAsync.WithRequest<ListAuditlogQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.AuditLogRoute.AuditLog)]
    [SwaggerOperation(Tags = [Router.AuditLogRoute.Tags], Summary = "List audit log")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromQuery] ListAuditlogQuery request,
        CancellationToken cancellationToken = default
    )
    {
        return this.Ok200(await sender.Send(request, cancellationToken));
    }
}
