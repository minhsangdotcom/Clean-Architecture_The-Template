using System.Data.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediaUpdateService<User> mediaUpdateService,
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

        string? key = mediaUpdateService.GetKey(avatar);
        user.Avatar = await mediaUpdateService.UploadAvatarAsync(avatar, key);
        // update default claim
        user.UpdateDefaultUserClaims();

        try
        {
            DbTransaction transaction = await unitOfWork.CreateTransactionAsync(cancellationToken);

            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);

            List<UserClaim> customUserClaims = mapper.Map<List<UserClaim>>(
                command.User.UserClaims,
                opt =>
                {
                    opt.Items[nameof(UserClaim.Type)] = KindaUserClaimType.Custom;
                    opt.Items[nameof(UserClaim.UserId)] = user.Id;
                }
            );

            await userManagerService.UpdateUserAsync(
                user,
                command.User.Roles!,
                customUserClaims,
                transaction
            );
            await unitOfWork.CommitAsync(cancellationToken);

            await mediaUpdateService.DeleteAvatarAsync(oldAvatar);
            return mapper.Map<UpdateUserResponse>(user);
        }
        catch (Exception)
        {
            await mediaUpdateService.DeleteAvatarAsync(user.Avatar);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
