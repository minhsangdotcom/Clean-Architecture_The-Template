using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Validators;

public class RoleClaimValidator : AbstractValidator<RoleClaimModel>
{
    public RoleClaimValidator()
    {
        Message<RoleClaimModel> messageBuilder = Messager.Create<RoleClaimModel>(nameof(RoleClaim));
        RuleFor(x => x.ClaimType)
            .NotEmpty().WithMessage(
                messageBuilder
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Existence)
                    .Negative()
                    .BuildMessage()
            );
        
        RuleFor(x => x.ClaimValue)
            .NotEmpty().WithMessage(
                messageBuilder
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Existence)
                    .Negative()
                    .BuildMessage()
            );
    }
}