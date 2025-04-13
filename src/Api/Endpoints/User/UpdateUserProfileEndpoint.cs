using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Users.Commands.Profiles;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class UpdateUserProfileEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.Profile, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update profile of user",
                Description = "Returns the user profile",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<UpdateUserProfileCommand>();
    }

    private async Task<
        Results<Ok<ApiResponse<UpdateUserProfileResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromForm] UpdateUserProfileCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
