using Application.UseCases.Projections.Users;
using Domain.Aggregates.Users.Enums;
using Mediator;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserCommand : UserModel, IRequest<CreateUserResponse>
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public Gender? Gender { get; set; }

    public UserStatus Status { get; set; }

    public List<Ulid>? RoleIds { get; set; }

    public List<UserClaimModel>? Claims { get; set; }
}