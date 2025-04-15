using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Users.Commands.Update;
using Contracts.ApiWrapper;
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
) : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async ValueTask<Result<CreateUserResponse>> Handle(
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

        //* creating new user address
        mappingUser.UpdateAddress(
            new(
                province!.Name,
                province.Id,
                district!.Name,
                district.Id,
                commune?.Name,
                commune?.Id,
                command.Street!
            )
        );

        //* adding user avatar
        string? key = mediaUpdateService.GetKey(command.Avatar);
        mappingUser.Avatar = await mediaUpdateService.UploadAvatarAsync(command.Avatar, key);

        string? userAvatar = null;
        try
        {
            _ = await unitOfWork.BeginTransactionAsync(cancellationToken);

            User user = await unitOfWork
                .Repository<User>()
                .AddAsync(mappingUser, cancellationToken);
            userAvatar = user.Avatar;

            //* trigger event to create claims for user ** default claims is about infomation of user
            user.CreateDefaultUserClaims();
            await unitOfWork.SaveAsync(cancellationToken);

            //* adding custom claims like permissions ...etc
            List<UserClaim> customClaims =
                command.UserClaims?.ToListUserClaim(UserClaimType.Custom, user.Id) ?? [];
            await userManagerService.CreateAsync(user, command.Roles!, customClaims);

            await unitOfWork.CommitAsync(cancellationToken);

            CreateUserResponse? response = await unitOfWork
                .DynamicReadOnlyRepository<User>()
                .FindByConditionAsync(
                    new GetUserByIdSpecification(user.Id),
                    x => x.ToCreateUserResponse(),
                    cancellationToken
                );
            return Result<CreateUserResponse>.Success(response!);
        }
        catch (Exception)
        {
            await mediaUpdateService.DeleteAvatarAsync(userAvatar);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
