using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Validators;

public class RoleClaimValidator : AbstractValidator<RoleClaimModel>
{
    public RoleClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                Messager
                    .Create<RoleClaimModel>(nameof(RoleClaim))
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
                    .Message
            );

        RuleFor(x => x.ClaimValue)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                Messager
                    .Create<RoleClaimModel>(nameof(RoleClaim))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage()
                    .Message
            );
    }
}
