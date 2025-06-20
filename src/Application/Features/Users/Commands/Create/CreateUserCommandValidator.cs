using System.Text.RegularExpressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IHttpContextAccessorService httpContextAccessorService;
    private readonly IUserManagerService userManagerService;
    private readonly IRoleManagerService roleManagerService;

    public CreateUserCommandValidator(
        IUserManagerService userManagerService,
        IHttpContextAccessorService httpContextAccessorService,
        IRoleManagerService roleManagerService
    )
    {
        this.userManagerService = userManagerService;
        this.httpContextAccessorService = httpContextAccessorService;
        this.roleManagerService = roleManagerService;

        ApplyRules();
    }

    private void ApplyRules()
    {
        Include(new UserValidator(userManagerService, httpContextAccessorService));
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Username!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must((_, x) => x!.IsValidUsername())
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Username!)
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            )
            .MustAsync((username, cancellationToken) =>
                IsUsernameAvailableAsync(username!, cancellationToken: cancellationToken)
            )
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.Username)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must((_, x) => x!.IsValidPassword())
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Strong)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Gender!)
                    .Negative()
                    .Message(MessageType.AmongTheAllowedOptions)
                    .Build()
            );

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .IsInEnum()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Negative()
                    .Message(MessageType.AmongTheAllowedOptions)
                    .Build()
            );

        RuleFor(x => x.Roles)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.Distinct().Count() == x!.Count)
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Unique)
                    .Negative()
                    .Build()
            )
            .MustAsync((roles, _) => IsRolesAvailableAsync(roles!))
            .WithState(x =>
                Messenger
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Found)
                    .Negative()
                    .Build()
            );

        When(
            x => x.UserClaims != null,
            () =>
            {
                RuleForEach(x => x.UserClaims).SetValidator(new UserClaimValidator());

                RuleFor(x => x.UserClaims)
                    .Must(x =>
                        x!
                            .FindAll(x => x.Id == null)
                            .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                            .Count() == x.FindAll(x => x.Id == null).Count
                    )
                    .WithState(x =>
                        Messenger
                            .Create<User>()
                            .Property(x => x.UserClaims!)
                            .Message(MessageType.Unique)
                            .Negative()
                            .BuildMessage()
                    );
            }
        );
    }

    private async Task<bool> IsUsernameAvailableAsync(
        string username,
        Ulid? id = null,
        CancellationToken cancellationToken = default
    ) => !await userManagerService.User.AnyAsync(
        x => (!id.HasValue && x.Username == username) || (x.Id != id && x.Username == username),
        cancellationToken
    );

    private async Task<bool> IsRolesAvailableAsync(IEnumerable<Ulid> roles) =>
        await roleManagerService.Roles.CountAsync(x => roles.Contains(x.Id)) == roles.Count();
}