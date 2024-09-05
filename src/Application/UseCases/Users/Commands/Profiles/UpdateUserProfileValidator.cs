using FluentValidation;
namespace Application.UseCases.Users.Commands.Profiles;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileQuery>
{
    public UpdateUserProfileValidator()
    {
    }
}