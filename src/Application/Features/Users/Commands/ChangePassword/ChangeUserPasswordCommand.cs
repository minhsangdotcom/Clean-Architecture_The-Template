using Mediator;

namespace Application.Features.Users.Commands.ChangePassword;

public class ChangeUserPasswordCommand : IRequest
{
    public string? OldPassword { get; set; }

    public string? NewPassword { get; set; }
}
