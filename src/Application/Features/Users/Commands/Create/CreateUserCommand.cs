using Application.Features.Common.Payloads.Users;
using Application.Features.Common.Projections.Users;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users.Enums;
using Mediator;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommand : UserPayload, IRequest<Result<CreateUserResponse>>
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public Gender? Gender { get; set; }

    public UserStatus Status { get; set; }

    public List<Ulid>? Roles { get; set; }

    public List<UserClaimPayload>? UserClaims { get; set; }
}
