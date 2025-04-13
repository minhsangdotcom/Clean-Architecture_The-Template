using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Users.Commands.Update;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class UpdateUserEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.GetUpdateDelete, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = " Update user ✏️ 🧑‍💻",
                Description = "Updates the information of an existing user identified by their ID.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<UpdateUser>();
    }

    private async Task<Results<Ok<ApiResponse<UpdateUserResponse>>, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromForm] UpdateUser request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new UpdateUserCommand() { UserId = id.ToString(), User = request };
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}
