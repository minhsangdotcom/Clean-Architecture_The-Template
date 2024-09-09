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
    public async ValueTask<UpdateUserResponse> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .Repository<User>()
                .GetByConditionSpecificationAsync(
                    new GetUserByIdSpecification(Ulid.Parse(command.UserId))
                ) ?? throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");

        IFormFile? avatar = command.User!.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command.User, user);

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        try
        {
            await unitOfWork.CreateTransactionAsync();

            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);
            await userManagerService.UpdateUserAsync(
                user,
                command.User.RoleIds!,
                mapper.Map<List<UserClaimType>>(
                    command.User.Claims,
                    opt => opt.Items[nameof(UserClaimType.Type)] = KindaUserClaimType.Custom
                ),
                unitOfWork.Transaction
            );

            await unitOfWork.CommitAsync();

            await avatarUpdate.DeleteAvatarAsync(oldAvatar);
            return mapper.Map<UpdateUserResponse>(user);
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                await avatarUpdate.DeleteAvatarAsync(user.Avatar);
            }
            throw;
        }
    }
}
