using FluentValidation;

namespace Application.UseCases.Users.Commands.Login;

public class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
    }
}