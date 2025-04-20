using System.Text.RegularExpressions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Messages;

namespace Application.Features.Common.Validators.Users;

public partial class UserValidator : AbstractValidator<UserModel>
{
    private readonly IActionAccessorService accessorService;
    private readonly IUserManagerService userManagerService;

    public UserValidator(
        IUserManagerService userManagerService,
        IActionAccessorService accessorService
    )
    {
        this.accessorService = accessorService;
        this.userManagerService = userManagerService;
        ApplyRules();
    }

    private void ApplyRules()
    {
        _ = Ulid.TryParse(accessorService.Id, out Ulid id);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
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
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            )
            .MustAsync(
                (email, cancellationToken) => IsEmailAvailableAsync(email!, id, cancellationToken)
            )
            .When(
                _ => accessorService.GetHttpMethod() == HttpMethod.Put.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Existence)
                    .Build()
            )
            .MustAsync(
                (email, cancellationToken) =>
                    IsEmailAvailableAsync(email!, cancellationToken: cancellationToken)
            )
            .When(
                _ => accessorService.GetHttpMethod() == HttpMethod.Post.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x =>
            {
                Regex regex = PhoneValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ProvinceId)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(nameof(x.ProvinceId))
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.DistrictId)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(nameof(UserModel.DistrictId))
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(nameof(UserModel.Street))
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }

    private async Task<bool> IsEmailAvailableAsync(
        string email,
        Ulid? id = null,
        CancellationToken cancellationToken = default
    ) =>
        !await userManagerService.User.AnyAsync(
            x => (!id.HasValue && x.Email == email) || (x.Id != id && x.Email == email),
            cancellationToken
        );

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\+?\d{7,15}$")]
    private static partial Regex PhoneValidationRegex();
}
