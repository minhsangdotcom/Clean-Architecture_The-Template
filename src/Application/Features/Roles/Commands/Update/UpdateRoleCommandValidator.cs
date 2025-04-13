using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Roles;
using FluentValidation;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IActionAccessorService actionAccessorService
    )
    {
        Include(new RoleValidator(roleManagerService, actionAccessorService));
    }
}
