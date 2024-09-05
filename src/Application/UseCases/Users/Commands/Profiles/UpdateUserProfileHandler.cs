using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using AutoMapper;
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
    IAvatarUpdateService<User> avatarUpdate
) : IRequestHandler<UpdateUserProfileQuery, UpdateUserProfileResponse>
{
    public async ValueTask<UpdateUserProfileResponse> Handle(UpdateUserProfileQuery command, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>()
            .GetByConditionSpecificationAsync(new GetUserByIdWithoutIncludeSpecification(currentUser.Id ?? Ulid.Empty)) ??
                throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");

        IFormFile? avatar = command.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command, user);

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await avatarUpdate.DeleteAvatarAsync(oldAvatar);

        return (await unitOfWork.Repository<User>()
                     .GetByConditionSpecificationAsync<UpdateUserProfileResponse>(new GetUserByIdSpecification(user.Id)))!;
    }
}