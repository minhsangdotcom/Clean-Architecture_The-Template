using Application.UseCases.Projections.Users;
using Application.UseCases.Users.Commands.Create;
using Contracts.Common.Messages;
using FluentValidation;

namespace Application.UseCases.Validators;

public class UserClaimValidator : AbstractValidator<UserClaimModel>
{
    public UserClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UserClaimModel>(nameof(CreateUserCommand.Claims))
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
                    .Create<UserClaimModel>(nameof(CreateUserCommand.Claims))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }    
}
