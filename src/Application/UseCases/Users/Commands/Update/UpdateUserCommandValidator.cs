using Application.UseCases.Validators;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.User)
            .SetValidator(new UserValidator()!);
    }
}