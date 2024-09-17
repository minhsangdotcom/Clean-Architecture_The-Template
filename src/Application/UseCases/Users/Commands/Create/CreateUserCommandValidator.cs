using System.Text.RegularExpressions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Commands.Create;

public partial class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(
        IUnitOfWork unitOfWork,
        IActionAccessorService accessorService
    )
    {
        Include(new UserValidator(unitOfWork, accessorService));

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.UserName!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(
                (_, x) =>
                {
                    Regex regex = UserNamValidationRegex();
                    return regex.IsMatch(x!);
                }
            )
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.UserName!)
                    .Message(MessageType.ValidFormat)
                    .Negative()
                    .Build()
            )
            .MustAsync((userName, _) => IsExistedUsername(unitOfWork, userName!))
            .WithState(x =>
                 Messager
                    .Create<User>()
                    .Property(x => x.UserName)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(
                (_, x) =>
                {
                    Regex regex = PassowrdValidationRegex();
                    return regex.IsMatch(x!);
                }
            )
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Password!)
                    .Message(MessageType.Strong)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Gender!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .IsInEnum()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Gender!)
                    .Message(MessageType.OuttaOption)
                    .Build()
            );

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .IsInEnum()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.Status!)
                    .Message(MessageType.OuttaOption)
                    .Build()
            );

        RuleFor(x => x.RoleIds)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.RoleIds!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.Distinct().Count() == x!.Count)
            .WithState(x =>
                Messager
                    .Create<CreateUserCommand>(nameof(User))
                    .Property(x => x.RoleIds!)
                    .Message(MessageType.Unique)
                    .Negative()
                    .Build()
            );
    
        When(
            x => x.Claims != null,
            () =>
            {
                RuleForEach(x => x.Claims).SetValidator(new UserClaimValidator());

                RuleFor(x => x.Claims)
                    .Must(x =>
                        x!
                            .FindAll(x => x.Id == null)
                            .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                            .Count() == x.FindAll(x => x.Id == null).Count
                    )
                    .WithState(x =>
                        Messager
                            .Create<CreateUserCommand>(nameof(User))
                            .Property(x => x.Claims!)
                            .Message(MessageType.Unique)
                            .Negative()
                            .BuildMessage()
                    );
            }
        );
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_.]+$")]
    private static partial Regex UserNamValidationRegex();

    [GeneratedRegex(@"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{7,})\S$")]
    private static partial Regex PassowrdValidationRegex();

    private static async Task<bool> IsExistedUsername(IUnitOfWork unitOfWork, string userName, Ulid? id = null)
    {
        return !await unitOfWork
            .Repository<User>()
            .AnyAsync(x =>
                (!id.HasValue && EF.Functions.ILike(x.UserName, userName))
                || (x.Id != id && EF.Functions.ILike(x.UserName, userName))
            );
    } 
}
