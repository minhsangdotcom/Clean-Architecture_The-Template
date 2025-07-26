using Application.Common.Extensions;
using Domain.Aggregates.Users;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.ChangePassword;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<ChangeUserPasswordCommand>(nameof(User))
                    .Property(x => x.OldPassword!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<ChangeUserPasswordCommand>(nameof(User))
                    .Property(x => x.NewPassword!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.IsValidPassword())
            .WithState(x =>
                Messenger
                    .Create<ChangeUserPasswordCommand>(nameof(User))
                    .Property(x => x.NewPassword!)
                    .Message(MessageType.Strong)
                    .Negative()
                    .Build()
            );
    }
}
