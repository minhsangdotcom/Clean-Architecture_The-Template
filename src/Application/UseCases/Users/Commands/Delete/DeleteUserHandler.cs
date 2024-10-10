using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Identity;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Commands.Delete;

public class DeleteUserHandler(IUnitOfWork unitOfWork, IMediaUpdateService<User> MediaUpdateService) : IRequestHandler<DeleteUserCommand>
{
    public async ValueTask<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>()
            .GetByConditionSpecificationAsync(new GetUserByIdWithoutIncludeSpecification(command.UserId)) ??
                throw new NotFoundException(
                     [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
                );
        string? avatar = user.Avatar;
        await unitOfWork.Repository<User>().DeleteAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await MediaUpdateService.DeleteAvatarAsync(avatar);
        return Unit.Value;
    }
}