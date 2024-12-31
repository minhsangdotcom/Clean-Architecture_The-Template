using Application.Features.Common.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentValidation;

namespace Application.Features.Common.Validators.Roles;

public class RoleClaimValidator : AbstractValidator<RoleClaimModel>
{
    public RoleClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .Cascade(CascadeMode.Stop)
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
            .Cascade(CascadeMode.Stop)
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
