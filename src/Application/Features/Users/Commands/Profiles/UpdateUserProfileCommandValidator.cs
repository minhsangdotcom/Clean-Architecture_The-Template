using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Validators.Users;
using FluentValidation;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator(
        IUserManagerService userManagerService,
        IActionAccessorService accessorService
    )
    {
        Include(new UserValidator(userManagerService, accessorService));
    }
}
