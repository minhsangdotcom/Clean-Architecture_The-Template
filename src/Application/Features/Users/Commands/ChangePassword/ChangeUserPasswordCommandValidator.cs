using System.Text.RegularExpressions;
using Domain.Aggregates.Users;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.ChangePassword;

public partial class ChangeUserPasswordCommandValidator
    : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithState(x =>
                Messager
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
                Messager
                    .Create<ChangeUserPasswordCommand>(nameof(User))
                    .Property(x => x.NewPassword!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x =>
            {
                Regex regex = PassowordValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<ChangeUserPasswordCommand>(nameof(User))
                    .Property(x => x.NewPassword!)
                    .Message(MessageType.Strong)
                    .Negative()
                    .Build()
            );
    }

    [GeneratedRegex(@"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{8,})\S$")]
    private static partial Regex PassowordValidationRegex();
}
