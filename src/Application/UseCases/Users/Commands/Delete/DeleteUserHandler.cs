using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.Users;
using Mediator;
using web.Specification.Specs;

namespace Application.UseCases.Users.Commands.Delete;

public class DeleteUserHandler(IUnitOfWork unitOfWork, IAvatarUpdateService<User> avatarUpdateService) : IRequestHandler<DeleteUserCommand>
{
    public async ValueTask<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>()
            .GetByConditionSpecificationAsync(new GetUserByIdWithoutIncludeSpecification(command.UserId)) ??
                throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");
        string? avatar = user.Avatar;
        await unitOfWork.Repository<User>().DeleteAsync(user);

        await avatarUpdateService.DeleteAvatarAsync(avatar);

        return Unit.Value;
    }
}