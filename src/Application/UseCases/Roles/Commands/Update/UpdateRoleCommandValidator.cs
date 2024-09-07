using Application.Common.Interfaces.Services;
using Application.UseCases.Validators;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(IRoleManagerService roleManagerService, IActionAccessorService actionAccessorService)
    {
        RuleFor(x => x.Role)
            .SetValidator(new RoleValidator(roleManagerService, actionAccessorService));
    }
} 