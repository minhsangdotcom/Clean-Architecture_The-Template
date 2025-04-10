using Application.Common.Errors;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.ResetPassword;

public class ResetUserPasswordHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ResetUserPasswordCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        ResetUserPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdIncludeResetPassword(command.UserId),
                cancellationToken
            );

        if (user == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    "The resource is not found",
                    Messager.Create<User>().Message(MessageType.Found).Negative().Build()
                )
            );
        }

        IEnumerable<UserResetPassword> resetPasswords = user.UserResetPasswords ?? [];
        UserResetPassword? resetPassword = resetPasswords.FirstOrDefault(x =>
            x.Token == command.Token
        );

        if (resetPassword == null)
        {
            return Result<string>.Failure(
                new BadRequestError(
                    "Error has occured with reset password token",
                    Messager
                        .Create<UserResetPassword>()
                        .Property(x => x.Token)
                        .Message(MessageType.Correct)
                        .Negative()
                        .Build()
                )
            );
        }

        if (resetPassword.Expiry <= DateTimeOffset.UtcNow)
        {
            return Result<string>.Failure(
                new BadRequestError(
                    "Error has occured with reset password token",
                    Messager
                        .Create<UserResetPassword>()
                        .Property(x => x.Token)
                        .Message(MessageType.Expired)
                        .Build()
                )
            );
        }

        if (user.Status == UserStatus.Inactive)
        {
            return Result<string>.Failure(
                new BadRequestError(
                    "Error has occured with current user",
                    Messager.Create<User>().Message(MessageType.Active).Negative().Build()
                )
            );
        }

        user.SetPassword(HashPassword(command.Password));

        await unitOfWork.Repository<UserResetPassword>().DeleteRangeAsync(resetPasswords);
        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return Result<string>.Success();
    }
}
