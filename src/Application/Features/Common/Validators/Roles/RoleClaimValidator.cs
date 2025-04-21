using Application.Features.Common.Projections.Roles;
using Domain.Aggregates.Roles;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Common.Validators.Roles;

public class RoleClaimValidator : AbstractValidator<RoleClaimModel>
{
    public RoleClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<RoleClaim>(nameof(Role.RoleClaims))
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<RoleClaim>(nameof(Role.RoleClaims))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }
}
