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

        UserUpdateRequest updateData = command.UpdateData;

        IFormFile? avatar = updateData.Avatar;
        string? oldAvatar = user.Avatar;

        user.FromUpdateUser(updateData);

        Province? province = await unitOfWork
            .Repository<Province>()
            .FindByIdAsync(updateData.ProvinceId, cancellationToken);
        if (province == null)
        {
            return Result<UpdateUserResponse>.Failure<NotFoundError>(
                new(
                    "Resource is not found",
                    Messager
                        .Create<User>()
                        .Property(nameof(UserUpdateRequest.ProvinceId))
                        .Message(MessageType.Existence)
                        .Negative()
                        .Build()
                )
            );
        }

        District? district = await unitOfWork
            .Repository<District>()
            .FindByIdAsync(updateData.DistrictId, cancellationToken);
        if (district == null)
        {
            return Result<UpdateUserResponse>.Failure<NotFoundError>(
                new(
                    "Resource is not found",
                    Messager
                        .Create<User>()
                        .Property(nameof(updateData.DistrictId))
                        .Message(MessageType.Existence)
                        .Negative()
                        .Build()
                )
            );
        }

        Commune? commune = null;
        if (updateData.CommuneId.HasValue)
        {
            commune = await unitOfWork
                .Repository<Commune>()
                .FindByIdAsync(updateData.CommuneId.Value, cancellationToken);

            if (commune == null)
            {
                return Result<UpdateUserResponse>.Failure<NotFoundError>(
                    new(
                        "Resource is not found",
                        Messager
                            .Create<User>()
                            .Property(nameof(UserUpdateRequest.CommuneId))
                            .Message(MessageType.Existence)
                            .Negative()
                            .Build()
                    )
                );
            }
        }
        //* replace address
        user.UpdateAddress(
            new(
                province!.FullName,
                province.Id,
                district!.FullName,
                district.Id,
                commune?.FullName,
                commune?.Id,
                command.UpdateData.Street!
            )
        );

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
                updateData.UserClaims?.ToListUserClaim(UserClaimType.Custom, user.Id) ?? [];
            await userManagerService.UpdateAsync(user, updateData.Roles!, customUserClaims);

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
