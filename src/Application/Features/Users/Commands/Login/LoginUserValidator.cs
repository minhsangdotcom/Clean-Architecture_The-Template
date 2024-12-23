using FluentValidation;

namespace Application.Features.Users.Commands.Login;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator() { }
}
