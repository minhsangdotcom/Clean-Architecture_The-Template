using Application.Common.Interfaces.Services;
using Application.UseCases.Validators;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleCommandValidator(IRoleManagerService roleManagerService, IActionAccessorService actionAccessorService)
    {
        Include(new RoleValidator(roleManagerService, actionAccessorService));
    }
}