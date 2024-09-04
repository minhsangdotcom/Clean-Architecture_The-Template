using FluentValidation;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
    }
}