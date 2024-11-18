using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Validators;

public class RoleValidator : AbstractValidator<RoleModel>
{
    private readonly IRoleManagerService roleManagerService;
    private readonly IActionAccessorService actionAccessorService;

    public RoleValidator(
        IRoleManagerService roleManagerService,
        IActionAccessorService actionAccessorService
    )
    {
        this.roleManagerService = roleManagerService;
        this.actionAccessorService = actionAccessorService;
        ApplyRules();
    }

    private void ApplyRules()
    {
        _ = Ulid.TryParse(actionAccessorService.Id, out Ulid id);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Negative()
                    .Message(MessageType.Null)
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .Build()
            )
            .MustAsync(
                (name, cancellationToken) =>
                    IsExistedNameAsync(name, cancellationToken: cancellationToken)
            )
            .When(
                _ => actionAccessorService.GetHttpMethod() == HttpMethod.Post.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .Build()
            )
            .MustAsync((name, cancellationToken) => IsExistedNameAsync(name, id, cancellationToken))
            .When(
                _ => actionAccessorService.GetHttpMethod() == HttpMethod.Put.ToString(),
                ApplyConditionTo.CurrentValidator
            )
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null, ApplyConditionTo.CurrentValidator)
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.MaximumLength)
                    .Build()
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
                    .WithState(x =>
                        Messager
                            .Create<RoleModel>(nameof(Role))
                            .Property(x => x.Claims!)
                            .Message(MessageType.Unique)
                            .Negative()
                            .Build()
                    );

                RuleFor(x => x.Claims)
                    .MustAsync(
                        (roleClaim, _) =>
                            IsExistClaimAsync(
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
                    .WithState(x =>
                        Messager
                            .Create<RoleModel>(nameof(Role))
                            .Property(x => x.Claims!)
                            .Message(MessageType.Existence)
                            .Build()
                    );
            }
        );
    }

    private async Task<bool> IsExistedNameAsync(
        string name,
        Ulid? id = null,
        CancellationToken cancellationToken = default
    )
    {
        return !await roleManagerService.Roles.AnyAsync(
            x =>
                (!id.HasValue && EF.Functions.ILike(x.Name, name))
                || (x.Id != id && EF.Functions.ILike(x.Name, name)),
            cancellationToken
        );
    }

    private async Task<bool> IsExistClaimAsync(
        Ulid id,
        IEnumerable<KeyValuePair<string, string>> roleClaims
    ) => !await roleManagerService.HasClaimInRoleAsync(id, roleClaims);
}
