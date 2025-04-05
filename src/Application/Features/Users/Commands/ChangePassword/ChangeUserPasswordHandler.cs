using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.ChangePassword;

public class ChangeUserPasswordHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<ChangeUserPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        Ulid? userId = currentUser.Id;

        User user =
            await unitOfWork
                .SpecificationRepository<User>()
                .FindByConditionAsync(
                    new GetUserByIdWithoutIncludeSpecification(userId ?? Ulid.Empty),
                    cancellationToken
                )
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().Build()]
            );

        if (!Verify(request.OldPassword, user.Password))
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<ChangeUserPasswordCommand>(nameof(User))
                        .Property(x => x.OldPassword!)
                        .Message(MessageType.Correct)
                        .Negative()
                        .Build(),
                ]
            );
        }

        user.SetPassword(HashPassword(request.NewPassword));

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}
