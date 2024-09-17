using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Commands.ResetPassword;

public class ResetUserPasswordHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ResetUserPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        ResetUserPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .Repository<User>()
                .GetByConditionSpecificationAsync(
                    new GetUserByIdIncludeResetPassword(command.UserId)
                )
            ?? throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().Build()]
            );

        IEnumerable<UserResetPassword> resetPasswords = user.UserResetPasswords ?? [];
        UserResetPassword? resetPassword =
            resetPasswords.FirstOrDefault(x => x.Token == command.Token)
            ?? throw new BadRequestException(
                [
                    Messager
                        .Create<UserResetPassword>()
                        .Property(x => x.Token)
                        .Message(MessageType.Correct)
                        .Negative()
                        .Build(),
                ]
            );

        if (resetPassword.Expiry <= DateTimeOffset.UtcNow)
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<UserResetPassword>()
                        .Property(x => x.Token)
                        .Message(MessageType.Expired)
                        .Build(),
                ]
            );
        }

        if (user.Status == UserStatus.DeActive)
        {
            throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Active).Negative().Build()]
            );
        }

        user.SetPassword(HashPassword(command.Password));

        await unitOfWork.Repository<UserResetPassword>().DeleteRangeAsync(resetPasswords);
        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}
