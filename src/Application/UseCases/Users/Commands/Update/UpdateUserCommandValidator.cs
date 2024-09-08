using Application.UseCases.Projections.Users;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.User)
            .NotEmpty().WithMessage(
                Messager.Create<UpdateUserCommand>()
                    .Property(x => x.User!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .SetValidator(new UserValidator()!);
    }
}