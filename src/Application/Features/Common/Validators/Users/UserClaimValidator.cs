using Application.Features.Common.Projections.Users;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.Features.Common.Validators.Users;

public class UserClaimValidator : AbstractValidator<UserClaimModel>
{
    public UserClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .Cascade(CascadeMode.Stop)
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
            .Cascade(CascadeMode.Stop)
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
