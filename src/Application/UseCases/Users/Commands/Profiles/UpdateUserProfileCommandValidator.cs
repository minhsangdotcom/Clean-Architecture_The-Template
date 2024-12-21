using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.UnitOfWorks;
using Application.UseCases.Validators.Users;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Profiles;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator(
        IUnitOfWork unitOfWork,
        IActionAccessorService accessorService
    )
    {
        Include(new UserValidator(unitOfWork, accessorService));
    }
}
