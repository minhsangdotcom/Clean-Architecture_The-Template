using Mediator;

namespace Application.Features.Users.Commands.Token;

public class RefreshUserTokenCommand : IRequest<RefreshUserTokenResponse>
{
    public string? RefreshToken { get; set; }
}
