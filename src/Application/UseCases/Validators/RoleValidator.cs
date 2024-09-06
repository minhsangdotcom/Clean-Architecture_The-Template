using Application.UseCases.Projections.Roles;
using FluentValidation;

namespace Application.UseCases.Validators;

public class RoleValidator : AbstractValidator<RoleModel>
{
    public RoleValidator()
    {
    }
}