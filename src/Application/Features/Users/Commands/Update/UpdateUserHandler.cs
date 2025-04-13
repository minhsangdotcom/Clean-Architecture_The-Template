using Application.Common.Errors;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMediaUpdateService<User> mediaUpdateService,
    IUserManagerService userManagerService
) : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    public async ValueTask<Result<UpdateUserResponse>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdSpecification(Ulid.Parse(command.UserId)),
                cancellationToken
            );

        if (user == null)
        {
            return Result<UpdateUserResponse>.Failure(
                new NotFoundError(
                    "Your resource is not found",
                    Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()
                )
            );
        }

        IFormFile? avatar = command.UpdateData!.Avatar;
        string? oldAvatar = user.Avatar;

        user.FromUpdateUser(command.UpdateData);

        Province? province = await unitOfWork
            .Repository<Province>()
            .FindByIdAsync(command.UpdateData.ProvinceId, cancellationToken);
        District? district = await unitOfWork
            .Repository<District>()
            .FindByIdAsync(command.UpdateData.DistrictId, cancellationToken);

        Commune? commune = null;
        if (command.UpdateData.CommuneId.HasValue)
        {
            commune = await unitOfWork
                .Repository<Commune>()
                .FindByIdAsync(command.UpdateData.CommuneId.Value, cancellationToken);
        }
        //* replace address
        user.UpdateAddress(new(province!, district!, commune, command.UpdateData.Street!));

        string? key = mediaUpdateService.GetKey(avatar);
        user.Avatar = await mediaUpdateService.UploadAvatarAsync(avatar, key);

        //* trigger event to update default claims -  that's infomation of user
        user.UpdateDefaultUserClaims();

        try
        {
            _ = await unitOfWork.BeginTransactionAsync(cancellationToken);

            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);

            //* update custom claims of user like permissions ...
            List<UserClaim> customUserClaims =
                command.UpdateData.UserClaims?.ToListUserClaim(KindaUserClaimType.Custom, user.Id)
                ?? [];
            await userManagerService.UpdateAsync(user, command.UpdateData.Roles!, customUserClaims);

            await unitOfWork.CommitAsync(cancellationToken);

            await mediaUpdateService.DeleteAvatarAsync(oldAvatar);
            return Result<UpdateUserResponse>.Success(user.ToUpdateUserResponse());
        }
        catch (Exception)
        {
            await mediaUpdateService.DeleteAvatarAsync(user.Avatar);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
