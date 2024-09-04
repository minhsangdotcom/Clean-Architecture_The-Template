using Application.Common.Interfaces.Repositories;
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

        if (
            await unitOfWork.Repository<User>().AnyAsync(x => true)
        )
        {
            return;
        }

        try
        {
            User user = new("Chloe", "Kim", "chloe.kim", HashPassword(Credential.UserDefaultPassword), "chloe.kim@gmail.com", "0925123123")
            {
                Address = "NYC",
                DayOfBirth = new DateTime(1990, 10, 1),
                Status = UserStatus.Active,
            };

            await unitOfWork.Repository<User>().AddAsync(user);
            await unitOfWork.SaveAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}