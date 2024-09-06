using Application.Common.Interfaces.Services;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleCommandValidator(IRoleManagerService roleManagerService, IActionAccessorService actionAccessorService)
    {
        var messageBuilder = Messager.Create<UpdateRole>(nameof(Role));
        Include(new RoleValidator(roleManagerService));
        When(x => x.RoleClaims != null, () =>
        {
            RuleFor(x => x.RoleClaims)
                .MustAsync(async (roleClaim, CancellationToken) =>
                    await roleManagerService.HasClaimInRoleAsync(
                        Ulid.Parse(actionAccessorService.Id),
                        roleClaim!.ToDictionary(x => x.ClaimType!, x => x.ClaimValue!)
                )
            ).WithMessage(
                messageBuilder
                    .Property(x => x.RoleClaims!)
                    .Message(MessageType.Existence)
                    .BuildMessage()
            );

            RuleFor(x => x.RoleClaims)
                .Must(x => x!
                        .FindAll(x => x.Id == null)
                        .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                        .Count() == x.FindAll(x => x.Id == null).Count
                )
                .WithMessage(
                    messageBuilder
                        .Property(x => x.RoleClaims!)
                        .Message(MessageType.NonUnique)
                        .BuildMessage()
                );
        });
    }
}