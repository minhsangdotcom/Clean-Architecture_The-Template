using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper,
    IMediaUpdateService<User> avatarUpdate
) : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResponse>
{
    public async ValueTask<UpdateUserProfileResponse> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .Repository<User>()
                .FindByConditionAsync(
                    new GetUserByIdWithoutIncludeSpecification(currentUser.Id ?? Ulid.Empty),
                    cancellationToken
                )
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        IFormFile? avatar = command.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command, user);

        Province? province = await unitOfWork
            .Repository<Province>()
            .FindByIdAsync(command.ProvinceId, cancellationToken);
        District? district = await unitOfWork
            .Repository<District>()
            .FindByIdAsync(command.DistrictId, cancellationToken);

        Commune? commune = null;
        if (command.CommuneId.HasValue)
        {
            commune = await unitOfWork
                .Repository<Commune>()
                .FindByIdAsync(command.CommuneId.Value, cancellationToken);
        }
        user.UpdateAddress(new(province!, district!, commune, command.Street!));

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        try
        {
            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);
            await avatarUpdate.DeleteAvatarAsync(oldAvatar);
        }
        catch (Exception)
        {
            await avatarUpdate.DeleteAvatarAsync(user.Avatar);
            throw;
        }

        return (
            await unitOfWork
                .Repository<User>()
                .FindByConditionAsync<UpdateUserProfileResponse>(
                    new GetUserByIdSpecification(user.Id),
                    cancellationToken
                )
        )!;
    }
}
