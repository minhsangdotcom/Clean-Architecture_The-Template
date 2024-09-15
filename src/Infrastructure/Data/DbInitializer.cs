using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Infrastructure.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public class DbInitializer
{
    public static async Task Initialize(IServiceProvider provider)
    {
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var roleManagerService = provider.GetRequiredService<IRoleManagerService>();
        var userManagerService = provider.GetRequiredService<IUserManagerService>();

        if (await unitOfWork.Repository<User>().AnyAsync(x => true))
        {
            return;
        }

        try
        {
            await unitOfWork.CreateTransactionAsync();

            User user =
                new(
                    "Chloe",
                    "Kim",
                    "chloe.kim",
                    HashPassword(Credential.USER_DEFAULT_PASSWORD),
                    "chloe.kim@gmail.com",
                    "0925123123"
                )
                {
                    Address = "NYC",
                    DayOfBirth = new DateTime(1990, 10, 1),
                    Status = UserStatus.Active,
                };

            Role role =
                new()
                {
                    Id = Ulid.Parse(Credential.ADMIN_ROLE_ID),
                    Name = Credential.ADMIN_ROLE,
                    RoleClaims = Credential
                        .CLAIMS.Select(x => new RoleClaim
                        {
                            ClaimType = x.Key,
                            ClaimValue = x.Value,
                        })
                        .ToList(),
                };

            await unitOfWork.Repository<User>().AddAsync(user);
            await unitOfWork.SaveAsync();

            await roleManagerService.CreateRoleAsync(role);
            await userManagerService.CreateUserAsync(
                user,
                [role.Id],
                user.GetUserClaims(),
                new(unitOfWork.Transaction!, unitOfWork.Connection!)
            );

            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}
