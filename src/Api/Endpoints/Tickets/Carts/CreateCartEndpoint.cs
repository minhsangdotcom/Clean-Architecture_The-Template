using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Tickets.Carts.Create;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Tickets.Carts;

public class CreateCartEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.CartRoute.Carts, HandleAsync)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Create cart 👮",
                Tags = [new OpenApiTag() { Name = Router.CartRoute.Carts }],
            });
    }

    private async Task<Results<Ok<ApiResponse<CreateCartResponse>>, ProblemHttpResult>> HandleAsync(
        [FromBody] CreateCartCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
