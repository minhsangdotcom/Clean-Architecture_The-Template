using System.Data.Common;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Users.Commands.Update;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.Features.Users.Commands.Create;

public class CreateUserHandler(
    IUnitOfWork unitOfWork,
    IMediaUpdateService<User> mediaUpdateService,
    IUserManagerService userManagerService
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async ValueTask<CreateUserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User mappingUser = command.ToUser();

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

        mappingUser.UpdateAddress(new(province!, district!, commune, command.Street!));

        string? key = mediaUpdateService.GetKey(command.Avatar);
        mappingUser.Avatar = await mediaUpdateService.UploadAvatarAsync(command.Avatar, key);

        string? userAvatar = null;
        try
        {
            DbTransaction transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            User user = await unitOfWork
                .Repository<User>()
                .AddAsync(mappingUser, cancellationToken);
            userAvatar = user.Avatar;

            // add default claims
            user.CreateDefaultUserClaims();
            await unitOfWork.SaveAsync(cancellationToken);

            List<UserClaim> customClaims =
                command.UserClaims?.ToListUserClaim(KindaUserClaimType.Custom, user.Id) ?? [];

            await userManagerService.CreateUserAsync(
                user,
                command.Roles!,
                customClaims,
                transaction
            );

            await unitOfWork.CommitAsync(cancellationToken);

            return (
                await unitOfWork
                    .SpecificationRepository<User>()
                    .FindByConditionAsync(
                        new GetUserByIdSpecification(user.Id),
                        x => x.ToCreateUserResponse(),
                        cancellationToken
                    )
            )!;
        }
        catch (Exception)
        {
            await mediaUpdateService.DeleteAvatarAsync(userAvatar);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
