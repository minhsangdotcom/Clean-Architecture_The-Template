using Application.UseCases.Users.Commands.Create;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class CreateUserEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<CreateUserCommand>.WithActionResult<ApiResponse>
{
    [HttpPost(Router.UserRoute.Users)]
    [SwaggerOperation(
            Tags = [Router.UserRoute.Tags],
            Summary = "create User"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromForm] CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        CreateUserResponse user = await sender.Send(request, cancellationToken);

        return this.Ok200(user);
    }
}