using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Common.Validators.Users;
using Domain.Aggregates.Users;
using FluentValidation;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(
        IUnitOfWork unitOfWork,
        IActionAccessorService accessorService
    )
    {
        _ = Ulid.TryParse(accessorService.Id, out Ulid id);

        RuleFor(x => x.User)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UpdateUserCommand>()
                    .Property(x => x.User!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .SetValidator(new UserValidator(unitOfWork, accessorService)!);

        RuleFor(x => x.User!.Roles)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UpdateUser>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        When(
            x => x.User!.UserClaims != null,
            () =>
            {
                RuleForEach(x => x.User!.UserClaims).SetValidator(new UserClaimValidator());
            }
        );
    }
}
