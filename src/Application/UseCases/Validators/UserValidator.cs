using System.Text.RegularExpressions;
using Application.UseCases.Projections.Users;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Validators;

public partial class UserValidator : AbstractValidator<UserModel>
{
    public UserValidator()
    {
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            );

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            );

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .Must(x =>
            {
                Regex regex = EmailValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .BuildMessage()
            );

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .Must(x =>
            {
                Regex regex = VietnamesePhoneValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .BuildMessage()
            );

        RuleFor(x => x.Address)
            .MaximumLength(1000)
            .When(x => x.Address != null)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Address!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
                    .Message
            );
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
    private static partial Regex VietnamesePhoneValidationRegex();
}
