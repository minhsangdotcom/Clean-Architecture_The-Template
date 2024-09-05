using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAvatarUpdateService<User> avatarUpdate,
    IUserManagerService userManagerService
) : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    public async ValueTask<UpdateUserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>()
               .GetByConditionSpecificationAsync(new GetUserByIdSpecification(Ulid.Parse(command.UserId))) ??
               throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");

        IFormFile? avatar = command.User!.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command.User, user);

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        // update role for user
        await userManagerService.UpdateRolesToUserAsync(user, command.User.RoleIds!);

        // update default user claim
        var claims = user.GetUserClaims().ToDictionary(x => x.ClaimType!, x => x.ClaimValue!);
        await userManagerService.ReplaceDefaultClaimsToUserAsync(user, claims);

        // update custom user claim
        await userManagerService.UpdateClaimsToUserAsync(
            user,
            mapper.Map<List<UserClaimType>>(command.User.Claims, opt => opt.Items[nameof(UserClaimType.Type)] = KindaUserClaimType.Custom)
        );

        await avatarUpdate.DeleteAvatarAsync(oldAvatar);

        return mapper.Map<UpdateUserResponse>(user);
    }
}