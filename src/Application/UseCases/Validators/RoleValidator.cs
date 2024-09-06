using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Roles;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Validators;

public class RoleValidator : AbstractValidator<RoleModel>
{
    public RoleValidator(IRoleManagerService roleManagerService)
    {
        Message<Role> messageBuilder = Messager.Create<Role>();

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
            ).MustAsync((name, CancellationToken) => roleManagerService.Roles.AnyAsync(x => x.Name == name,CancellationToken))
            .WithMessage(
                messageBuilder
                    .Property(x => x.Name)
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
    }
}