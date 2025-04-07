using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Delete;

public class DeleteUserHandler(IUnitOfWork unitOfWork, IMediaUpdateService<User> mediaUpdateService)
    : IRequestHandler<DeleteUserCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .DynamicRepository<User>()
                .FindByConditionAsync(
                    new GetUserByIdWithoutIncludeSpecification(command.UserId),
                    cancellationToken
                )
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );
        string? avatar = user.Avatar;
        await unitOfWork.Repository<User>().DeleteAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await mediaUpdateService.DeleteAvatarAsync(avatar);
        return Unit.Value;
    }
}
