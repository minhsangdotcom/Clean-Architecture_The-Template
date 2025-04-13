using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Users.Queries.Detail;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class GetUserDetailEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.UserRoute.GetUpdateDelete, HandleAsync)
            .WithName(Router.UserRoute.GetRouteName)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get detail of existed user",
                Description = "Returns the user if found",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            });
    }

    private async Task<
        Results<Ok<ApiResponse<GetUserDetailResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new GetUserDetailQuery(Ulid.Parse(id));
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}
