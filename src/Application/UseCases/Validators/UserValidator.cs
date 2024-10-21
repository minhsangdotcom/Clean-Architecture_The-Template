using System.Text.RegularExpressions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Users;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Validators;

public partial class UserValidator : AbstractValidator<UserModel>
{
    public UserValidator(IUnitOfWork unitOfWork, IActionAccessorService accessorService)
    {
        _ = Ulid.TryParse(accessorService.Id, out Ulid id);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.LastName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .MaximumLength(256)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.FirstName)
                    .Message(MessageType.MaximumLength)
                    .Build()
            );

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x =>
            {
                Regex regex = EmailValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            )
            .MustAsync((email, _) => IsExistedEmail(unitOfWork, email!, id))
            .When(_ => accessorService.GetHttpMethod() == HttpMethod.Put.ToString())
            .WithState(x =>
                 Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Existence)
                    .Build()
            )
            .MustAsync((email, _) => IsExistedEmail(unitOfWork, email!))
            .When(_ => accessorService.GetHttpMethod() == HttpMethod.Post.ToString())
            .WithState(x =>
                 Messager
                    .Create<User>()
                    .Property(x => x.Email)
                    .Message(MessageType.Existence)
                    .Build()
            );

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x =>
            {
                Regex regex = VietnamesePhoneValidationRegex();
                return regex.IsMatch(x!);
            })
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.PhoneNumber)
                    .Message(MessageType.Valid)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Address)
            .MaximumLength(1000)
            .When(x => x.Address != null)
            .WithState(x =>
                Messager
                    .Create<User>()
                    .Property(x => x.Address!)
                    .Message(MessageType.MaximumLength)
                    .Build()
                    .Message
            );
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
    private static partial Regex VietnamesePhoneValidationRegex();

    private static async Task<bool> IsExistedEmail(IUnitOfWork unitOfWork, string email, Ulid? id = null)
    {
        return !await unitOfWork
            .Repository<User>()
            .AnyAsync(x =>
                (!id.HasValue && EF.Functions.ILike(x.Email, email))
                || (x.Id != id && EF.Functions.ILike(x.Email, email))
            );
    }
}
