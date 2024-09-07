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
            .NotEmpty().WithMessage(
                Messager.Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .MaximumLength(256).WithMessage(
                Messager.Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            );

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(
                Messager.Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
        )
        .MaximumLength(256).WithMessage(
                Messager.Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
        );

         RuleFor(x => x.Email)
            .NotEmpty().WithMessage(
                Messager.Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .Must(x =>
            {
                Regex regex = EmailValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage(
                Messager.Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .BuildMessage()
            );

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(
                 Messager.Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
            )
            .Must(x =>
            {
                Regex regex = VietnamesePhoneValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage(Messager.Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .BuildMessage()
            );

        RuleFor(x => x.Address)
            .MaximumLength(1000)
            .When(x => x.Address != null)
            .WithMessage(
                Messager.Create<User>()
                    .Property(x => x.Address!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            );
    }

    
    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
    private static partial Regex VietnamesePhoneValidationRegex();
}