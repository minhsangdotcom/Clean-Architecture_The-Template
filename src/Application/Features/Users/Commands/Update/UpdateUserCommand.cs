using Application.Features.Common.Projections.Users;
using Contracts.ApiWrapper;
using Contracts.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
{
    public string UserId { get; set; } = string.Empty;

    public UserUpdateRequest UpdateData { get; set; } = null!;
}

public class UserUpdateRequest : UserModel
{
    public List<Ulid>? Roles { get; set; }

    public List<UserClaimModel>? UserClaims { get; set; }
}
