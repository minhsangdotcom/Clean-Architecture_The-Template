using Application.Common.Interfaces.Services;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(IRoleManagerService roleManagerService)
    {
        Include(new RoleValidator(roleManagerService));

        RuleFor(x => x.RoleClaims)
            .Must(x => x!
                    .FindAll(x => x.Id == null)
                    .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                    .Count() == x.FindAll(x => x.Id == null).Count
            ).When(x => x.RoleClaims != null, ApplyConditionTo.CurrentValidator)
            .WithMessage(
                Messager.Create<CreateRoleCommand>(nameof(Role))
                    .Property(x => x.RoleClaims!)
                    .Message(MessageType.NonUnique)
                    .BuildMessage()
            );
    }
}