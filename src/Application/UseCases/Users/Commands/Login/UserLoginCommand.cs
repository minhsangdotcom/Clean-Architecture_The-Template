using Mediator;

namespace Application.UseCases.Users.Commands.Login;

public class UserLoginCommand : IRequest<UserLoginResponse>
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}