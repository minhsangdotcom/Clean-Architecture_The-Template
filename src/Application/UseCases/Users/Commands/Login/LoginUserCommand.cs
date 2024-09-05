using Mediator;

namespace Application.UseCases.Users.Commands.Login;

public class LoginUserCommand : IRequest<LoginUserResponse>
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}