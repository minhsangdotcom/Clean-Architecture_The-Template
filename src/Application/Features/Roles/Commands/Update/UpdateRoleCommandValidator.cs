using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Roles;
using Contracts.Common.Messages;
using FluentValidation;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IActionAccessorService actionAccessorService
    )
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UpdateRoleCommand>()
                    .Property(x => x.Role!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .SetValidator(new RoleValidator(roleManagerService, actionAccessorService));
    }
}
