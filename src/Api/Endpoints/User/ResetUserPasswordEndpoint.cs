using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Users.Commands.ResetPassword;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class ResetUserPasswordEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.ResetPassword, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Reset user password 🔄 🔑",
                Description =
                    "Resets a user's password using a valid token from a password reset request.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            }).WithRequestValidation<UpdateUserPassword>();
    }

    private async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromBody] UpdateUserPassword request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(
            new ResetUserPasswordCommand { UserId = id, UpdateUserPassword = request },
            cancellationToken
        );
        return result.ToNoContentResult();
    }
}
