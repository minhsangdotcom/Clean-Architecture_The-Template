using Application.Common.Errors;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Delete;

public class DeleteUserHandler(IUnitOfWork unitOfWork, IMediaUpdateService<User> mediaUpdateService)
    : IRequestHandler<DeleteUserCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdWithoutIncludeSpecification(command.UserId),
                cancellationToken
            );

        if (user == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    "Resource is not found",
                    Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()
                )
            );
        }
        string? avatar = user.Avatar;
        await unitOfWork.Repository<User>().DeleteAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await mediaUpdateService.DeleteAvatarAsync(avatar);
        return Result<string>.Success();
    }
}
