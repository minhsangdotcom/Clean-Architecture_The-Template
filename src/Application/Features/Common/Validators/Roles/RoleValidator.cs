using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Projections.Roles;
using CaseConverter;
using Domain.Aggregates.Roles;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Messages;

namespace Application.Features.Common.Validators.Roles;

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
                    IsNameAvailableAsync(name, cancellationToken: cancellationToken)
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
            .MustAsync(
                (name, cancellationToken) => IsNameAvailableAsync(name, id, cancellationToken)
            )
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
            .MaximumLength(1000)
            .When(x => x.Description != null, ApplyConditionTo.CurrentValidator)
            .WithState(x =>
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Description!)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        When(
            x => x.RoleClaims != null,
            () =>
            {
                RuleForEach(x => x.RoleClaims).SetValidator(new RoleClaimValidator());
            }
        );
    }

    private async Task<bool> IsNameAvailableAsync(
        string name,
        Ulid? id = null,
        CancellationToken cancellationToken = default
    )
    {
        string caseName = name.ToSnakeCase();
        return !await roleManagerService.Roles.AnyAsync(
            x => (!id.HasValue && x.Name == caseName) || (x.Id != id && x.Name == caseName),
            cancellationToken
        );
    }
}
