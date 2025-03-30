using System.Text.Json;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Create;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture
{
    public async Task<UserAddress> SeedingRegionsAsync()
    {
        using var scope = factory!.Services.CreateScope();
        var provider = scope.ServiceProvider;
        IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

        if (
            await unitOfWork.Repository<Province>().AnyAsync()
            && await unitOfWork.Repository<District>().AnyAsync()
            && await unitOfWork.Repository<Commune>().AnyAsync()
        )
        {
            return GetDefaultAddress();
        }

        string path = AppContext.BaseDirectory;
        string fullPath = Path.Combine(path, "Data", "Seeds");

        string provinceFilePath = Path.Combine(fullPath, "Provinces.json");
        IEnumerable<Province>? provinces = Read<Province>(provinceFilePath);

        string districtFilePath = Path.Combine(fullPath, "Districts.json");
        IEnumerable<District>? districts = Read<District>(districtFilePath);

        string communeFilePath = Path.Combine(fullPath, "Wards.json");
        IEnumerable<Commune>? communes = Read<Commune>(communeFilePath);

        Province province = provinces?.FirstOrDefault(x => x.Code == "79")!;
        District district = districts?.FirstOrDefault(x => x.Code == "783")!;
        Commune commune = communes?.FirstOrDefault(x => x.Code == "27508")!;

        //HCM
        await unitOfWork.Repository<Province>().AddAsync(province);
        // Cu Chi
        await unitOfWork.Repository<District>().AddAsync(district);
        await unitOfWork.Repository<Commune>().AddAsync(commune);

        await unitOfWork.SaveAsync();

        return new(province.Id, district.Id, commune.Id);
    }

    public async Task<User> CreateAdminUserAsync(
        UserAddress? address = null,
        IFormFile? avatar = null
    )
    {
        address ??= GetDefaultAddress();
        Role role = await CreateAdminRoleAsync();
        CreateUserCommand command =
            new()
            {
                FirstName = "admin",
                LastName = "super",
                Username = "super.admin",
                Password = "Admin@123",
                Email = "super.amdin@gmail.com",
                DayOfBirth = DateTime.UtcNow,
                PhoneNumber = "0925123321",
                Gender = Gender.Male,
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                CommuneId = address.CommuneId,
                Roles = [role.Id],
                Street = "abcdef",
                Status = UserStatus.Active,
                Avatar = avatar,
                UserClaims =
                [
                    new UserClaimModel() { ClaimType = "test1", ClaimValue = "test1.value" },
                    new UserClaimModel() { ClaimType = "test2", ClaimValue = "test2.value" },
                    new UserClaimModel() { ClaimType = "test3", ClaimValue = "test3.value" },
                ],
            };

        return await CreateUserAsync(command);
    }

    public async Task<User> CreateManagerUserAsync(
        UserAddress? address = null,
        IFormFile? avatar = null
    )
    {
        address ??= GetDefaultAddress();
        Role role = await CreateManagerRoleAsync();
        CreateUserCommand command =
            new()
            {
                FirstName = "Steave",
                LastName = "Roger",
                Username = "steave.Roger",
                Password = "Admin@123",
                Email = "steave.roger@gmail.com",
                DayOfBirth = DateTime.UtcNow,
                PhoneNumber = "0925321321",
                Gender = Gender.Male,
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                CommuneId = address.CommuneId,
                Roles = [role.Id],
                Street = "abcdef",
                Status = UserStatus.Active,
                Avatar = avatar,
                UserClaims =
                [
                    new UserClaimModel() { ClaimType = "test1", ClaimValue = "test1.value" },
                    new UserClaimModel() { ClaimType = "test2", ClaimValue = "test2.value" },
                    new UserClaimModel() { ClaimType = "test3", ClaimValue = "test3.value" },
                ],
            };

        return await CreateUserAsync(command);
    }

    public async Task<User> CreateNormalUserAsync(
        UserAddress? address = null,
        IFormFile? avatar = null
    )
    {
        address ??= GetDefaultAddress();
        Role role = await CreateNormalRoleAsync();
        CreateUserCommand command =
            new()
            {
                FirstName = "Sang",
                LastName = "Tran",
                Username = "sang.tran",
                Password = "Admin@123",
                Email = "sang.tran@gmail.com",
                DayOfBirth = DateTime.UtcNow,
                PhoneNumber = "0925123124",
                Gender = Gender.Male,
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                CommuneId = address.CommuneId,
                Roles = [role.Id],
                Street = "abcdef",
                Status = UserStatus.Active,
                Avatar = avatar,
                UserClaims =
                [
                    new UserClaimModel() { ClaimType = "test1", ClaimValue = "test1.value" },
                    new UserClaimModel() { ClaimType = "test2", ClaimValue = "test2.value" },
                    new UserClaimModel() { ClaimType = "test3", ClaimValue = "test3.value" },
                ],
            };

        return await CreateUserAsync(command);
    }

    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        CreateUserResponse createUserResponse = await SendAsync(command);
        return (await FindUserByIdAsync(createUserResponse.Id))!;
    }

    public async Task<User?> FindUserByIdAsync(Ulid userId)
    {
        using var scope = factory!.Services.CreateScope();
        IUnitOfWork? unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        return await unitOfWork!
            .Repository<User>()
            .FindByConditionAsync(new GetUserByIdSpecification(userId));
    }

    private static UserAddress GetDefaultAddress() =>
        new(
            Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"),
            Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"),
            Ulid.Parse("01JAZDXEATWDPJTD9DBS9MHB7M")
        );

    private static List<T>? Read<T>(string path)
        where T : class
    {
        using FileStream json = File.OpenRead(path);
        List<T>? datas = JsonSerializer.Deserialize<List<T>>(json);
        return datas;
    }
}

public record UserAddress(Ulid ProvinceId, Ulid DistrictId, Ulid CommuneId);
