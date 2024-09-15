using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediaUpdateService<User> avatarUpdate,
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
                )
            ?? throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );

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
                new(unitOfWork.Transaction!, unitOfWork.Connection!)
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
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}
