using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Validators;

public class RoleValidator : AbstractValidator<RoleModel>
{
    public RoleValidator(
        IRoleManagerService roleManagerService,
        IActionAccessorService actionAccessorService
    )
    {
        _ = Ulid.TryParse(actionAccessorService.Id, out Ulid id);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Negative()
                    .Message(MessageType.Null)
                    .BuildMessage()
                    .Message
            )
            .MaximumLength(256)
            .WithMessage(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
                    .Message
            )
            .MustAsync(
                (name, CancellationToken) =>
                    IsExistedNameAsync(roleManagerService, name, CancellationToken)
            )
            .When(_ => actionAccessorService.GetHttpMethod() == HttpMethod.Post.ToString())
            .WithMessage(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .BuildMessage()
                    .Message
            )
            .MustAsync(
                (name, CancellationToken) =>
                    IsExistedNameAsync(roleManagerService, name, CancellationToken, id)
            )
            .When(_ => actionAccessorService.GetHttpMethod() == HttpMethod.Put.ToString())
            .WithMessage(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .BuildMessage()
                    .Message
            );

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null, ApplyConditionTo.CurrentValidator)
            .WithMessage(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .BuildMessage()
                    .Message
            );

        When(
            x => x.Claims != null,
            () =>
            {
                RuleForEach(x => x.Claims).SetValidator(new RoleClaimValidator());

                RuleFor(x => x.Claims)
                    .Must(x =>
                        x!
                            .FindAll(x => x.Id == null)
                            .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                            .Count() == x.FindAll(x => x.Id == null).Count
                    )
                    .WithMessage(
                        Messager
                            .Create<RoleModel>(nameof(Role))
                            .Property(x => x.Claims!)
                            .Message(MessageType.Unique)
                            .Negative()
                            .BuildMessage()
                            .Message
                    );

                RuleFor(x => x.Claims)
                    .MustAsync(
                        (roleClaim, CancellationToken) =>
                            IsExistClaimAsync(
                                roleManagerService,
                                id,
                                roleClaim!
                                    .Where(x => x.Id == null)
                                    .Select(x => new KeyValuePair<string, string>(
                                        x.ClaimType!,
                                        x.ClaimValue!
                                    ))
                            )
                    )
                    .When(
                        _ => actionAccessorService.GetHttpMethod() == HttpMethod.Put.ToString(),
                        ApplyConditionTo.CurrentValidator
                    )
                    .WithMessage(
                        Messager
                            .Create<RoleModel>(nameof(Role))
                            .Property(x => x.Claims!)
                            .Message(MessageType.Existence)
                            .BuildMessage()
                            .Message
                    );
            }
        );
    }

    private static async Task<bool> IsExistedNameAsync(
        IRoleManagerService roleManagerService,
        string name,
        CancellationToken cancellationToken,
        Ulid? id = null
    )
    {
        return !await roleManagerService.Roles.AnyAsync(
            x =>
                (!id.HasValue && EF.Functions.ILike(x.Name, name))
                || (x.Id != id && EF.Functions.ILike(x.Name, name)),
            cancellationToken
        );
    }

    public static async Task<bool> IsExistClaimAsync(
        IRoleManagerService roleManagerService,
        Ulid id,
        IEnumerable<KeyValuePair<string, string>> roleClaims
    ) => !await roleManagerService.HasClaimInRoleAsync(id, roleClaims);
}
