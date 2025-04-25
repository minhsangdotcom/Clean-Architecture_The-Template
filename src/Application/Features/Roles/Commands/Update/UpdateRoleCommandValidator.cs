using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Roles;
using FluentValidation;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<RoleUpdateRequest>
{
    public UpdateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IHttpContextAccessorService httpContextAccessorService
    )
    {
        Include(new RoleValidator(roleManagerService, httpContextAccessorService));
    }
}
