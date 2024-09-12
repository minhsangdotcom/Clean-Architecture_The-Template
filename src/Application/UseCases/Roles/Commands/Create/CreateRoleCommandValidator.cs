using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Validators;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(IRoleManagerService roleManagerService , IActionAccessorService actionAccessorService)
    {
        Include(new RoleValidator(roleManagerService, actionAccessorService));
    }
}