using System.Data.Common;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.ValueObjects;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Data;

public class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider provider)
    {
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var roleManagerService = provider.GetRequiredService<IRoleManagerService>();
        var userManagerService = provider.GetRequiredService<IUserManagerService>();
        var logger = provider.GetRequiredService<ILogger>();

        if (await unitOfWork.Repository<User>().AnyAsync())
        {
            return;
        }

        logger.Information("Seeding data is starting.............");
        try
        {
            DbTransaction dbTransaction = await unitOfWork.CreateTransactionAsync();

            Role[] roles =
            [
                new()
                {
                    Id = Ulid.Parse(Credential.ADMIN_ROLE_ID),
                    Name = Credential.ADMIN_ROLE,
                    RoleClaims = Credential
                        .ADMIN_CLAIMS.Select(x => new RoleClaim()
                        {
                            ClaimType = x.Key,
                            ClaimValue = x.Value,
                        })
                        .ToList(),
                },
                new()
                {
                    Id = Ulid.Parse(Credential.MANAGER_ROLE_ID),
                    Name = Credential.MANAGER_ROLE,
                    RoleClaims = Credential
                        .MANAGER_CLAIMS.Select(x => new RoleClaim()
                        {
                            ClaimType = x.Key,
                            ClaimValue = x.Value,
                        })
                        .ToList(),
                },
            ];

            await roleManagerService.CreateRangeRoleAsync(roles);

            List<User> users = await UserData(unitOfWork);
            users.ForEach(x => x.CreateDefaultUserClaims());
            await unitOfWork.Repository<User>().AddRangeAsync(users);
            await unitOfWork.SaveAsync();

            foreach (var user in users)
            {
                await userManagerService.CreateUserAsync(
                    user,
                    [roles[new Random().Next(0, 2)].Id],
                    transaction: dbTransaction
                );
            }

            logger.Information("Seeding data has finished.............");
            await unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            logger.Information("error had occured while seeding data with {message}", ex);
            throw;
        }
    }

    private static async Task<GetRegionResult> GetRegionAsync(
        IUnitOfWork unitOfWork,
        string provinceCode,
        string districtCode,
        string communeCode
    )
    {
        Province? province = await unitOfWork
            .Repository<Province>()
            .ApplyQuery(x => x.Code == provinceCode)
            .FirstOrDefaultAsync();
        District? district = await unitOfWork
            .Repository<District>()
            .ApplyQuery(x => x.Code == districtCode)
            .FirstOrDefaultAsync();
        Commune? commune = await unitOfWork
            .Repository<Commune>()
            .ApplyQuery(x => x.Code == communeCode)
            .FirstOrDefaultAsync();
        return new(province, district, commune);
    }

    private static async Task<List<User>> UserData(IUnitOfWork unitOfWork)
    {
        string sg = "79";
        string hn = "01";
        string dn = "48";
        GetRegionResult region = await GetRegionAsync(unitOfWork, sg, "783", "27543");

        User user =
            new(
                "Chloe",
                "Kim",
                "chloe.kim",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "chloe.kim@gmail.com",
                "0925123123",
                new Address(region.Province!, region.District!, region.Commune, "132 Ham Nghi")
            )
            {
                DayOfBirth = new DateTime(1990, 10, 1),
                Status = UserStatus.Active,
                Gender = Gender.Female,
                Id = Credential.UserIds.CHLOE_KIM_ID,
            };

        GetRegionResult johnDoeRegion = await GetRegionAsync(unitOfWork, sg, "760", "26743");
        User johnDoe =
            new(
                "John",
                "Doe",
                "john.doe",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "john.doe@example.com",
                "0803456789",
                new Address(
                    johnDoeRegion.Province!,
                    johnDoeRegion.District!,
                    johnDoeRegion.Commune,
                    "136/9 Le Thanh Ton"
                )
            )
            {
                DayOfBirth = new DateTime(1985, 4, 23),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.JOHN_DOE_ID,
            };

        GetRegionResult aliceSmithRegion = await GetRegionAsync(unitOfWork, sg, "760", "26737");
        User aliceSmith =
            new(
                "Alice",
                "Smith",
                "alice.smith",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "alice.smith@example.com",
                "0912345678",
                new Address(
                    aliceSmithRegion.Province!,
                    aliceSmithRegion.District!,
                    aliceSmithRegion.Commune,
                    "95B Nguyen Van Thu"
                )
            )
            {
                DayOfBirth = new DateTime(1992, 7, 19),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.ALICE_SMITH_ID,
            };

        GetRegionResult bobJohnsonRegion = await GetRegionAsync(unitOfWork, sg, "760", "26758");
        User bobJohnson =
            new(
                "Bob",
                "Johnson",
                "bob.johnson",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "bob.johnson@example.com",
                "0934567890",
                new Address(
                    bobJohnsonRegion.Province!,
                    bobJohnsonRegion.District!,
                    bobJohnsonRegion.Commune,
                    "18 Truong Son"
                )
            )
            {
                DayOfBirth = new DateTime(1980, 3, 15),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.BOB_JOHNSON_ID,
            };

        GetRegionResult emilyBrownRegion = await GetRegionAsync(unitOfWork, sg, "760", "26746");
        User emilyBrown =
            new(
                "Emily",
                "Brown",
                "emily.brown",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "emily.brown@example.com",
                "0945678901",
                new Address(
                    emilyBrownRegion.Province!,
                    emilyBrownRegion.District!,
                    emilyBrownRegion.Commune,
                    "46 Nam Ky Khoi Nghia"
                )
            )
            {
                DayOfBirth = new DateTime(1995, 5, 5),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.EMILY_BROWN_ID,
            };

        GetRegionResult jamesWilliamsRegion = await GetRegionAsync(unitOfWork, sg, "771", "27163");
        User jamesWilliams =
            new(
                "James",
                "Williams",
                "james.williams",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "james.williams@example.com",
                "0978901234",
                new Address(
                    jamesWilliamsRegion.Province!,
                    jamesWilliamsRegion.District!,
                    jamesWilliamsRegion.Commune,
                    "595/29A3 Cach Mang Thang 8"
                )
            )
            {
                DayOfBirth = new DateTime(1983, 11, 9),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.JAMES_WILLIAMS_ID,
            };

        GetRegionResult oliviaTaylorRegion = await GetRegionAsync(unitOfWork, sg, "771", "27172");
        User oliviaTaylor =
            new(
                "Olivia",
                "Taylor",
                "olivia.taylor",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "olivia.taylor@example.com",
                "0989012345",
                new Address(
                    oliviaTaylorRegion.Province!,
                    oliviaTaylorRegion.District!,
                    oliviaTaylorRegion.Commune,
                    "175B Cao Thang"
                )
            )
            {
                DayOfBirth = new DateTime(1998, 2, 18),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.OLIVIA_TAYLOR_ID,
            };

        GetRegionResult danielLeeRegion = await GetRegionAsync(unitOfWork, sg, "771", "27196");
        User danielLee =
            new(
                "Daniel",
                "Lee",
                "daniel.lee",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "daniel.lee@example.com",
                "0901234567",
                new Address(
                    danielLeeRegion.Province!,
                    danielLeeRegion.District!,
                    danielLeeRegion.Commune,
                    "102 Ly Thuong Kiet"
                )
            )
            {
                DayOfBirth = new DateTime(1987, 9, 21),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.DANIEL_LEE_ID,
            };

        GetRegionResult sophiaGarciaRegion = await GetRegionAsync(unitOfWork, sg, "771", "27166");
        User sophiaGarcia =
            new(
                "Sophia",
                "Garcia",
                "sophia.garcia",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "sophia.garcia@example.com",
                "0912345679",
                new Address(
                    sophiaGarciaRegion.Province!,
                    sophiaGarciaRegion.District!,
                    sophiaGarciaRegion.Commune,
                    "51 To Hien Thanh"
                )
            )
            {
                DayOfBirth = new DateTime(1994, 12, 12),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.SHOPHIA_GARCIA_ID,
            };

        GetRegionResult michaelMartinezRegion = await GetRegionAsync(
            unitOfWork,
            sg,
            "766",
            "27001"
        );
        User michaelMartinez =
            new(
                "Michael",
                "Martinez",
                "michael.martinez",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "michael.martinez@example.com",
                "0913456789",
                new Address(
                    michaelMartinezRegion.Province!,
                    michaelMartinezRegion.District!,
                    michaelMartinezRegion.Commune,
                    "78 Au Co"
                )
            )
            {
                DayOfBirth = new DateTime(1978, 8, 8),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.MICHAEL_MARTINEZ_ID,
            };

        GetRegionResult isabellaHarrisRegion = await GetRegionAsync(unitOfWork, sg, "766", "27004");
        User isabellaHarris =
            new(
                "Isabella",
                "Harris",
                "isabella.harris",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "isabella.harris@example.com",
                "0945678902",
                new Address(
                    isabellaHarrisRegion.Province!,
                    isabellaHarrisRegion.District!,
                    isabellaHarrisRegion.Commune,
                    "40 Nguyen Hong Dao"
                )
            )
            {
                DayOfBirth = new DateTime(1991, 1, 1),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.ISABELLA_HARRIS_ID,
            };

        GetRegionResult davidClarkRegion = await GetRegionAsync(unitOfWork, sg, "766", "26986");
        User davidClark =
            new(
                "David",
                "Clark",
                "david.clark",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "david.clark@example.com",
                "0934567891",
                new Address(
                    davidClarkRegion.Province!,
                    davidClarkRegion.District!,
                    davidClarkRegion.Commune,
                    "66 Van Coi"
                )
            )
            {
                DayOfBirth = new DateTime(1984, 6, 6),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.DAVID_CLARK_ID,
            };

        GetRegionResult emmaRodriguezRegion = await GetRegionAsync(unitOfWork, hn, "001", "00007");
        User emmaRodriguez =
            new(
                "Emma",
                "Rodriguez",
                "emma.rodriguez",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "emma.rodriguez@example.com",
                "0956789012",
                new Address(
                    emmaRodriguezRegion.Province!,
                    emmaRodriguezRegion.District!,
                    emmaRodriguezRegion.Commune,
                    "178 Phan Ke Binh"
                )
            )
            {
                DayOfBirth = new DateTime(1993, 3, 3),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.EMMA_RODRIGUEZ_ID,
            };

        GetRegionResult andrewMooreRegion = await GetRegionAsync(unitOfWork, hn, "001", "00006");
        User andrewMoore =
            new(
                "Andrew",
                "Moore",
                "andrew.moore",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "andrew.moore@example.com",
                "0923456789",
                new Address(
                    andrewMooreRegion.Province!,
                    andrewMooreRegion.District!,
                    andrewMooreRegion.Commune,
                    "6 Doi Nhan"
                )
            )
            {
                DayOfBirth = new DateTime(1981, 10, 30),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.ANDREW_MOORE_ID,
            };

        GetRegionResult avaJacksonRegion = await GetRegionAsync(unitOfWork, hn, "001", "00025");
        User avaJackson =
            new(
                "Ava",
                "Jackson",
                "ava.jackson",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "ava.jackson@example.com",
                "0935678903",
                new Address(
                    avaJacksonRegion.Province!,
                    avaJacksonRegion.District!,
                    avaJacksonRegion.Commune,
                    "545 kim Ma"
                )
            )
            {
                DayOfBirth = new DateTime(2000, 4, 14),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.AVA_JACKSON_ID,
            };

        GetRegionResult joshuaWhiteRegion = await GetRegionAsync(unitOfWork, hn, "002", "00076");
        User joshuaWhite =
            new(
                "Joshua",
                "White",
                "joshua.white",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "joshua.white@example.com",
                "0914567890",
                new Address(
                    joshuaWhiteRegion.Province!,
                    joshuaWhiteRegion.District!,
                    joshuaWhiteRegion.Commune,
                    "9 Tong Duy Tan"
                )
            )
            {
                DayOfBirth = new DateTime(1986, 11, 17),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.JOSHUA_WHITE_ID,
            };

        GetRegionResult charlotteThomasRegion = await GetRegionAsync(
            unitOfWork,
            hn,
            "002",
            "00070"
        );
        User charlotteThomas =
            new(
                "Charlotte",
                "Thomas",
                "charlotte.thomas",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "charlotte.thomas@example.com",
                "0934567892",
                new Address(
                    charlotteThomasRegion.Province!,
                    charlotteThomasRegion.District!,
                    charlotteThomasRegion.Commune,
                    "10 Ly Quoc Su"
                )
            )
            {
                DayOfBirth = new DateTime(1997, 7, 7),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.CHARLOTTE_THOMAS_ID,
            };

        GetRegionResult ethanKingRegion = await GetRegionAsync(unitOfWork, dn, "495", "20306");
        User ethanKing =
            new(
                "Ethan",
                "King",
                "ethan.king",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "ethan.king@example.com",
                "0923456781",
                new Address(
                    ethanKingRegion.Province!,
                    ethanKingRegion.District!,
                    ethanKingRegion.Commune,
                    "35 Dinh Liet, Hoa An"
                )
            )
            {
                DayOfBirth = new DateTime(1999, 9, 9),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.ETHAN_KING_ID,
            };

        GetRegionResult abigailScottRegion = await GetRegionAsync(unitOfWork, dn, "495", "20314");
        User abigailScott =
            new(
                "Abigail",
                "Scott",
                "abigail.scott",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "abigail.scott@example.com",
                "0916789013",
                new Address(
                    abigailScottRegion.Province!,
                    abigailScottRegion.District!,
                    abigailScottRegion.Commune,
                    "55 Mac Dang Doanh"
                )
            )
            {
                DayOfBirth = new DateTime(1989, 2, 2),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.ABIGAIL_SCOTT_ID,
            };

        GetRegionResult liamPerezRegion = await GetRegionAsync(unitOfWork, dn, "495", "20305");
        User liamPerez =
            new(
                "Liam",
                "Perez",
                "liam.perez",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "liam.perez@example.com",
                "0909876543",
                new Address(
                    liamPerezRegion.Province!,
                    liamPerezRegion.District!,
                    liamPerezRegion.Commune,
                    "131/7 Le Trong Tan"
                )
            )
            {
                DayOfBirth = new DateTime(1988, 12, 25),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
                Id = Credential.UserIds.LIAM_PEREZ_ID,
            };

        return
        [
            user,
            johnDoe,
            aliceSmith,
            bobJohnson,
            emilyBrown,
            jamesWilliams,
            oliviaTaylor,
            danielLee,
            sophiaGarcia,
            michaelMartinez,
            isabellaHarris,
            davidClark,
            emmaRodriguez,
            andrewMoore,
            avaJackson,
            joshuaWhite,
            charlotteThomas,
            ethanKing,
            abigailScott,
            liamPerez,
        ];
    }
}

internal record GetRegionResult(Province? Province, District? District, Commune? Commune);
