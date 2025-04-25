using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Roles;
using FluentValidation;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IHttpContextAccessorService httpContextAccessorService
    )
    {
        Include(new RoleValidator(roleManagerService, httpContextAccessorService));
    }
}
