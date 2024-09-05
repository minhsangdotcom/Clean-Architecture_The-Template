using Mediator;

namespace Application.UseCases.Users.Commands.Token;

public class UserTokenCommand : IRequest<UserTokenResponse>
{
    public string? RefreshToken { get; set; }
}