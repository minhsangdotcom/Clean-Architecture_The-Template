using Application.Common.Exceptions;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using Domain.Aggregates.Regions;

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
                .FindByConditionAsync(
                    new GetUserByIdSpecification(Ulid.Parse(command.UserId)),
                    cancellationToken
                )
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        IFormFile? avatar = command.User!.Avatar;
        string? oldAvatar = user.Avatar;

        mapper.Map(command.User, user);

        Province? province = await unitOfWork
            .Repository<Province>()
            .FindByIdAsync(command.User.ProvinceId, cancellationToken);
        District? district = await unitOfWork
            .Repository<District>()
            .FindByIdAsync(command.User.DistrictId, cancellationToken);

        Commune? commune = null;
        if (command.User.CommuneId.HasValue)
        {
            commune = await unitOfWork
                .Repository<Commune>()
                .FindByIdAsync(command.User.CommuneId.Value, cancellationToken);
        }
        user.UpdateAddress(new(province!, district!, commune, command.User.Street!));

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
                mapper.Map<IEnumerable<UserClaim>>(
                    command.User.Claims,
                    opt => {
                        opt.Items[nameof(UserClaim.Type)] = KindaUserClaimType.Custom;
                        opt.Items[nameof(UserClaim.UserId)] = user.Id;
                    }
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
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}
