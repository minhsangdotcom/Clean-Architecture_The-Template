using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Create;

public partial class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.LastName).ToUpper()}_REQUIRED");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.FirstName).ToUpper()}_REQUIRED");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.UserName).ToUpper()}_REQUIRED")
            .Must((_, x) =>
            {
                Regex regex = UserNamValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage($"{nameof(CreateUserCommand.UserName).ToUpper()}_INVALID_FORMAT");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.Password).ToUpper()}_REQUIRED")
            .Must((_, x) =>
            {
                Regex regex = PassowrdValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage($"{nameof(CreateUserCommand.Password).ToUpper()}_NOT_STRONG_ENGOUGH");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.Email).ToUpper()}_REQUIRED")
            .Must(x =>
            {
                Regex regex = EmailValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage($"{nameof(CreateUserCommand.Email).ToUpper()}_INVALID_FORMAT");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.PhoneNumber).ToUpper()}_REQUIRED")
            .Must(x =>
            {
                Regex regex = VietnamesePhoneValidationRegex();
                return regex.IsMatch(x!);
            }).WithMessage($"{nameof(CreateUserCommand.PhoneNumber).ToUpper()}_INVALID_FORMAT");


        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.Gender).ToUpper()}_REQUIRED")
            .IsInEnum().WithMessage($"{nameof(CreateUserCommand.Gender).ToUpper()}_OUTTA_OPTION");

         RuleFor(x => x.Status)
            .NotEmpty().WithMessage($"{nameof(CreateUserCommand.Status).ToUpper()}_REQUIRED")
            .IsInEnum().WithMessage($"{nameof(CreateUserCommand.Status).ToUpper()}_OUTTA_OPTION");
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_.]+$")]
    private static partial Regex UserNamValidationRegex();

    [GeneratedRegex(@"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{7,})\S$")]
    private static partial Regex PassowrdValidationRegex();

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
    private static partial Regex VietnamesePhoneValidationRegex();
}