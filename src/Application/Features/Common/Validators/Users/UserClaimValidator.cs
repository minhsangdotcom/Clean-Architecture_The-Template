using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Common.Validators.Users;

public class UserClaimValidator : AbstractValidator<UserClaimModel>
{
    public UserClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UserClaim>(nameof(User.UserClaims))
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UserClaim>(nameof(User.UserClaims))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }
}
