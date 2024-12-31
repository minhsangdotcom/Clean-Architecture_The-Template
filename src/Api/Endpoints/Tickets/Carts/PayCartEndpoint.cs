using Application.Common.Interfaces.Services.DistributedCache;
using Application.Features.Tickets.Carts.Pays;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Tickets.Carts;

public class PayCartEndpoint(IQueueService queueService)
    : EndpointBaseAsync.WithRequest<PayCartRequest>.WithActionResult<ApiResponse>
{
    [HttpPost(Router.CartRoute.Pay)]
    [SwaggerOperation(Tags = [Router.CartRoute.Tags], Summary = "pay cart")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        PayCartRequest request,
        CancellationToken cancellationToken = default
    )
    {
        bool result = await queueService.EnqueueAsync(request);

        return this.Ok200(result);
    }
}
