using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediaUpdateService<User> MediaUpdateService,
    IUserManagerService userManagerService
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async ValueTask<CreateUserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User userMapping = mapper.Map<User>(command);

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

        userMapping.UpdateAddress(new(province!, district!, commune, command.Street!));

        string? key = MediaUpdateService.GetKey(command.Avatar);
        userMapping.Avatar = await MediaUpdateService.UploadAvatarAsync(command.Avatar, key);

        try
        {
            await unitOfWork.CreateTransactionAsync();

            User user = await unitOfWork
                .Repository<User>()
                .AddAsync(userMapping, cancellationToken);

            // add default claims
            user.CreateDefaultUserClaims();
            await unitOfWork.SaveAsync(cancellationToken);

            IEnumerable<UserClaim> customClaims = mapper.Map<List<UserClaim>>(
                command.Claims,
                opt =>
                {
                    opt.Items[nameof(UserClaim.Type)] = KindaUserClaimType.Custom;
                    opt.Items[nameof(UserClaim.UserId)] = user.Id;
                }
            );

            await userManagerService.CreateUserAsync(
                user,
                command.RoleIds!,
                customClaims,
                unitOfWork.Transaction
            );

            await unitOfWork.CommitAsync();

            return (
                await unitOfWork
                    .Repository<User>()
                    .FindByConditionAsync<CreateUserResponse>(
                        new GetUserByIdSpecification(user.Id),
                        cancellationToken
                    )
            )!;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}
