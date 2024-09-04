using Application.UseCases.Users.Commands.Update;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.User;

public class UpdateUserEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<UpdateUserCommand>.WithActionResult<ApiResponse>
{
    [HttpPut(Router.UserRoute.GetUpdateDelete)]
    [SwaggerOperation(
            Tags = [Router.UserRoute.Tags],
            Summary = "Update User"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(command, cancellationToken));
}