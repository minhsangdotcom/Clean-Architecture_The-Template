using Api.common.EndpointConfigurations;
using Api.common.Routers;
using Application.Common.Interfaces.Services.Queue;
using Application.Features.Tickets.Carts.Pays;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Tickets.Carts;

public class PayCartEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.CartRoute.Pay, HandleAsync)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Pay cart",
                Tags = [new OpenApiTag() { Name = Router.CartRoute.Carts }],
            });
    }

    private async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromBody] CartData data,
        [FromServices] IQueueService queueService,
        CancellationToken cancellationToken = default
    )
    {
        _ = await queueService.EnqueueAsync(
            new PayCartRequest() { CartId = id, CustomerId = data.CustomerId }
        );
        return TypedResults.NoContent();
    }
}
