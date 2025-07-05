using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UserUpdateRequest>
{
    public UpdateUserCommandValidator(
        IUserManagerService userManagerService,
        IHttpContextAccessorService httpContextAccessorService,
        ICurrentUser currentUser
    )
    {
        Include(new UserValidator(userManagerService, httpContextAccessorService,currentUser)!);

        RuleFor(x => x.Roles)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<UserUpdateRequest>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        When(
            x => x.UserClaims != null,
            () =>
            {
                RuleForEach(x => x!.UserClaims).SetValidator(new UserClaimValidator());
            }
        );
    }
}
