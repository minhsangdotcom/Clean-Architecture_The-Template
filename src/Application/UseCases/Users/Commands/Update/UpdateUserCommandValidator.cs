using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Application.UseCases.Validators;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IActionAccessorService accessorService;
    private readonly IUserManagerService userManagerService;

    public UpdateUserCommandValidator(
        IUnitOfWork unitOfWork,
        IActionAccessorService accessorService,
        IUserManagerService userManagerService
    )
    {
        this.unitOfWork = unitOfWork;
        this.accessorService = accessorService;
        this.userManagerService = userManagerService;
        
        ApplyRules();
    }

    private void ApplyRules()
    {
        _ = Ulid.TryParse(accessorService.Id, out Ulid id);

        RuleFor(x => x.User)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<UpdateUserCommand>()
                    .Property(x => x.User!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .SetValidator(new UserValidator(unitOfWork, accessorService)!);

        When(
            x => x.User!.UserClaims != null,
            () =>
            {
                RuleForEach(x => x.User!.UserClaims).SetValidator(new UserClaimValidator());

                RuleFor(x => x.User!.UserClaims)
                    .Must(x =>
                        x!
                            .FindAll(x => x.Id == null)
                            .DistinctBy(x => new { x.ClaimType, x.ClaimValue })
                            .Count() == x.FindAll(x => x.Id == null).Count
                    )
                    .WithState(x =>
                        Messager
                            .Create<User>()
                            .Property(x => x.UserClaims!)
                            .Message(MessageType.Unique)
                            .Negative()
                            .Build()
                    );

                RuleFor(x => x.User!.UserClaims)
                    .MustAsync(
                        (roleClaim, CancellationToken) =>
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
                    .WithState(x =>
                        Messager
                            .Create<User>()
                            .Property(x => x.UserClaims!)
                            .Message(MessageType.Existence)
                            .Build()
                    );
            }
        );
    }

    public async Task<bool> IsExistClaimAsync(
        Ulid id,
        IEnumerable<KeyValuePair<string, string>> userClaims
    ) => !await userManagerService.HasClaimsInUserAsync(id, userClaims);
}
