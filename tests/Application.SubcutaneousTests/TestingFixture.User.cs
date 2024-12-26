using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Update;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture
{
    public async Task SeedingRegionsAsync()
    {
        using var scope = factory!.Services.CreateScope();
        var provider = scope.ServiceProvider;
        await RegionDataSeeding.SeedingAsync(provider);
    }

    public async Task<UpdateUserCommand> CreateUserAsync()
    {
        var response = await this.CreateRoleAsync("roleTest");

        CreateUserCommand command =
            new()
            {
                FirstName = "admin",
                LastName = "super",
                Username = "super.admin",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Email = "super.amdin@gmail.com",
                DayOfBirth = DateTime.UtcNow,
                PhoneNumber = "0925123321",
                Gender = Gender.Male,
                ProvinceId = Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"),
                DistrictId = Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"),
                CommuneId = Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"),
                Roles = [response.Id],
                Street = "abcdef",
                Status = UserStatus.Active,
                UserClaims =
                [
                    new UserClaimModel() { ClaimType = "test1", ClaimValue = "test1.value" },
                    new UserClaimModel() { ClaimType = "test2", ClaimValue = "test2.value" },
                    new UserClaimModel() { ClaimType = "test3", ClaimValue = "test3.value" },
                ],
            };

        CreateUserResponse createUserResponse = await SendAsync(command);

        return new()
        {
            UserId = createUserResponse.Id.ToString(),
            User = new UpdateUser()
            {
                FirstName = createUserResponse.FirstName,
                LastName = createUserResponse.LastName,
                Email = createUserResponse.Email,
                DayOfBirth = createUserResponse.DayOfBirth,
                PhoneNumber = createUserResponse.PhoneNumber,
                ProvinceId = createUserResponse.Province!.Id,
                DistrictId = createUserResponse.District!.Id,
                CommuneId = createUserResponse.Commune!.Id,
                Street = createUserResponse.Street,
                Roles = [.. createUserResponse.Roles!.Select(x => x.Id)],
                UserClaims =
                [
                    .. createUserResponse.UserClaims!.Select(x => new UserClaimModel()
                    {
                        ClaimType = x.ClaimType,
                        ClaimValue = x.ClaimValue,
                    }),
                ],
            },
        };
    }

    public async Task<User?> FindUserByIdAsync(Ulid userId)
    {
        using var scope = factory!.Services.CreateScope();
        IUnitOfWork? unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        return await unitOfWork!
            .Repository<User>()
            .FindByConditionAsync(new GetUserByIdSpecification(userId));
    }
}
