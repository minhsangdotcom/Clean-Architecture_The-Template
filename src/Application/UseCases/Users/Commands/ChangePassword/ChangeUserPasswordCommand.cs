using Mediator;

namespace Application.UseCases.Users.Commands.ChangePassword;

public class ChangeUserPasswordCommand : IRequest
{
    public string? OldPassword { get; set; }

    public string? NewPassword { get; set; }
}