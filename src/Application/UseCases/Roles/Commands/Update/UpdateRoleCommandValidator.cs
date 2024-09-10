using Application.Common.Interfaces.Services;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(IRoleManagerService roleManagerService, IActionAccessorService actionAccessorService)
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(
                Messager.Create<UpdateRoleCommand>()
                    .Property(x => x.Role!)
                    .Message(MessageType.Null)
                    .Negative()
                    .BuildMessage().Message
            )
            .SetValidator(new RoleValidator(roleManagerService, actionAccessorService));
    }
} 