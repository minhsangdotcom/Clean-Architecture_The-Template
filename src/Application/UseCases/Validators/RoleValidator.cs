using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Validators;

public class RoleValidator : AbstractValidator<RoleModel>
{
    public RoleValidator(IRoleManagerService roleManagerService, IActionAccessorService actionAccessorService)
    {
        Message<RoleModel> messageBuilder = Messager.Create<RoleModel>(nameof(Role));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(
                messageBuilder
                    .Property(x => x.Name!)
                    .Negative()
                    .Message(MessageType.Null)
                    .BuildMessage()
            )
            .MaximumLength(256).WithMessage(
                messageBuilder
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            ).MustAsync((name, CancellationToken) => IsExistedName(roleManagerService, name, CancellationToken))
            .WithMessage(
                messageBuilder
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .BuildMessage()
            );

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null, ApplyConditionTo.CurrentValidator)
            .WithMessage(
                messageBuilder
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
            );

        When(x => x.Claims != null, () =>
        {
            RuleFor(x => x.Claims)
                .MustAsync(async (roleClaim, CancellationToken) =>
                    await roleManagerService.HasClaimInRoleAsync(
                        Ulid.Parse(actionAccessorService.Id),
                        roleClaim!.ToDictionary(x => x.ClaimType!, x => x.ClaimValue!)
                )
            ).When(x => actionAccessorService.GetHttpMethod() == HttpMethod.Put.ToString(), ApplyConditionTo.CurrentValidator)
            .WithMessage(
                messageBuilder
                    .Property(x => x.Claims!)
                    .Message(MessageType.Existence)
                    .BuildMessage()
            );

            RuleFor(x => x.Claims)
                .Must(x => x!
                        .FindAll(x => x.Id == null)
                        .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                        .Count() == x.FindAll(x => x.Id == null).Count
                )
                .WithMessage(
                    messageBuilder
                        .Property(x => x.Claims!)
                        .Message(MessageType.NonUnique)
                        .BuildMessage()
                );

            RuleForEach(x => x.Claims)
                .SetValidator(new RoleClaimValidator());
        });
    }

    private static async Task<bool> IsExistedName(IRoleManagerService roleManagerService, string name, CancellationToken cancellationToken)
    {
        return !await roleManagerService.Roles.AnyAsync(x => EF.Functions.ILike(x.Name, name), cancellationToken);
    }
}