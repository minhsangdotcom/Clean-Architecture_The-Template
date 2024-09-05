using FluentValidation;

namespace Application.UseCases.Users.Commands.Login;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
    }
}