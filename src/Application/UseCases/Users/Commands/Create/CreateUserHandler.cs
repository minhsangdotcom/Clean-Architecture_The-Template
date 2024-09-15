using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAvatarUpdateService<User> avatarUpdateService,
    IUserManagerService userManagerService
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async ValueTask<CreateUserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User userCreation = mapper.Map<User>(command);

        string? key = avatarUpdateService.GetKey(command.Avatar);
        userCreation.Avatar = await avatarUpdateService.UploadAvatarAsync(command.Avatar, key);

        try
        {
            await unitOfWork.CreateTransactionAsync();

            User user = await unitOfWork.Repository<User>().AddAsync(userCreation);
            await unitOfWork.SaveAsync(cancellationToken);

            // update claim to user consist of default and custom claims
            var claims = user.GetUserClaims()
                .Concat(
                    mapper.Map<IEnumerable<UserClaimType>>(
                        command.Claims,
                        opt => opt.Items[nameof(UserClaimType.Type)] = KindaUserClaimType.Custom
                    )
                );

            await userManagerService.CreateUserAsync(
                user,
                [.. command.RoleIds!],
                claims,
                new(unitOfWork.Transaction!, unitOfWork.Connection!)
            );

            await unitOfWork.CommitAsync();

            return (
                await unitOfWork
                    .Repository<User>()
                    .GetByConditionSpecificationAsync<CreateUserResponse>(
                        new GetUserByIdSpecification(user.Id)
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
