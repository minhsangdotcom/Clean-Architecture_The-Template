using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using web.Specification.Specs;

namespace Application.UseCases.Users.Commands.Profiles;

public class UpdateUserProfileHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper,
    IMediaUpdateService<User> avatarUpdate
) : IRequestHandler<UpdateUserProfileQuery, UpdateUserProfileResponse>
{
    public async ValueTask<UpdateUserProfileResponse> Handle(
        UpdateUserProfileQuery command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .Repository<User>()
                .GetByConditionSpecificationAsync(
                    new GetUserByIdWithoutIncludeSpecification(currentUser.Id ?? Ulid.Empty)
                )
            ?? throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        IFormFile? avatar = command.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command, user);

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await avatarUpdate.DeleteAvatarAsync(oldAvatar);

        return (
            await unitOfWork
                .Repository<User>()
                .GetByConditionSpecificationAsync<UpdateUserProfileResponse>(
                    new GetUserByIdSpecification(user.Id)
                )
        )!;
    }
}
