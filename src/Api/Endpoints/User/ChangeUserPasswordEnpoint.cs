using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Users.Commands.ChangePassword;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.User;

public class ChangeUserPasswordEnpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.ChangePassword, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Change user password",
                Description = "Returns 204",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            });
    }

    private async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        [FromBody] ChangeUserPasswordCommand request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToNoContentResult();
    }
}
