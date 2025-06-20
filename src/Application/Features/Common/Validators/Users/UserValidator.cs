using Application.Common.Extensions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Payloads.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Messages;

namespace Application.Features.Common.Validators.Users;

public partial class UserValidator : AbstractValidator<UserPayload>
{
    private readonly IHttpContextAccessorService httpContextAccessorService;
    private readonly IUserManagerService userManagerService;

    public UserValidator(
        IUserManagerService userManagerService,
        IHttpContextAccessorService httpContextAccessorService
    )
    {
        this.httpContextAccessorService = httpContextAccessorService;
        this.userManagerService = userManagerService;
        ApplyRules();
    }

    private void ApplyRules()
    {
        _ = Ulid.TryParse(httpContextAccessorService.GetId(), out Ulid id);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.IsValidEmail())
            .WithState(x =>
                Messenger
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
                _ => httpContextAccessorService.GetHttpMethod() == HttpMethod.Put.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messenger
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
                _ => httpContextAccessorService.GetHttpMethod() == HttpMethod.Post.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.IsValidPhoneNumber())
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ProvinceId)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(nameof(x.ProvinceId))
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.DistrictId)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(nameof(UserPayload.DistrictId))
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(nameof(UserPayload.Street))
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
}
