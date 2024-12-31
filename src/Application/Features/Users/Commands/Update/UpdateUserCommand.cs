using Application.Features.Common.Projections.Users;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    [FromRoute(Name = Router.Id)]
    public string UserId { get; set; } = string.Empty;

    [FromForm]
    public UpdateUser? User { get; set; }
}

public class UpdateUser : UserModel
{
    public List<Ulid>? Roles { get; set; }

    public List<UserClaimModel>? UserClaims { get; set; }
}
