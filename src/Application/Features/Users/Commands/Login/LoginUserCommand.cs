using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Users.Commands.Login;

public class LoginUserCommand : IRequest<Result<LoginUserResponse>>
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}
