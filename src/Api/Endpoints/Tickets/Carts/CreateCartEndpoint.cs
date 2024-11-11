using Application.UseCases.Tickets.Carts.Create;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Tickets.Carts;

public class CreateCartEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<CreateCartCommand>.WithActionResult<ApiResponse>
{
    [HttpPost(Router.CartRoute.Cart)]
    [SwaggerOperation(Tags = [Router.CartRoute.Tags], Summary = "create cart")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        CreateCartCommand request,
        CancellationToken cancellationToken = default
    )
    {
        return this.Ok200(await sender.Send(request, cancellationToken));
    }
}
