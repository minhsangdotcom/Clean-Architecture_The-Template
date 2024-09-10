using System.Text.RegularExpressions;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Create;

public partial class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {

        Include(new UserValidator());

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.UserName!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage().Message
            )
            .Must((_, x) =>
            {
                Regex regex = UserNamValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.UserName!)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .BuildMessage().Message
            );

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage().Message
            )
            .Must((_, x) =>
            {
                Regex regex = PassowrdValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Strong)
                    .Negative()
                    .BuildMessage().Message
            );


        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage(
                 Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Gender!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage().Message
            )
            .IsInEnum().WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Gender!)
                    .Message(MessageType.OuttaOption)
                    .BuildMessage().Message
            );

         RuleFor(x => x.Status)
            .NotEmpty().WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage().Message
            )
            .IsInEnum().WithMessage(
                Messager.Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Message(MessageType.OuttaOption)
                    .BuildMessage().Message
            );
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_.]+$")]
    private static partial Regex UserNamValidationRegex();

    [GeneratedRegex(@"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{7,})\S$")]
    private static partial Regex PassowrdValidationRegex();
}