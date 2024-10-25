using System.Text;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Regions;
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
        var dbContext = provider.GetRequiredService<IDbContext>();
        var logger = provider.GetRequiredService<ILogger>();

        if (await unitOfWork.Repository<User>().AnyAsync(x => true))
        {
            return;
        }

        logger.Information("Seeding data is starting.............");
        try
        {
            await unitOfWork.CreateTransactionAsync();

            Role[] roles =
            [
                new()
                {
                    Id = Ulid.Parse(Credential.ADMIN_ROLE_ID),
                    Name = Credential.ADMIN_ROLE,
                    RoleClaims = Credential
                        .ADMIN_CLAIMS.Select(x => new RoleClaim
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
                        .MANAGER_CLAIMS.Select(x => new RoleClaim
                        {
                            ClaimType = x.Key,
                            ClaimValue = x.Value,
                        })
                        .ToList(),
                },
            ];

            await dbContext.Set<Role>().AddRangeAsync(roles);
            await dbContext.SaveChangesAsync();

            User[] users = await UserData(
                dbContext.Set<Province>(),
                dbContext.Set<District>(),
                dbContext.Set<Commune>()
            );
            await unitOfWork.Repository<User>().AddRangeAsync(users);
            await unitOfWork.SaveAsync();

            foreach (var user in users)
            {
                await userManagerService.CreateUserAsync(
                    user,
                    [roles[new Random().Next(0, 2)].Id],
                    user.GetUserClaims(),
                    unitOfWork.Transaction
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

    private static async Task<RandomRegionResult> RandomRegion(
        DbSet<Province> provinces,
        DbSet<District> districts,
        DbSet<Commune> communes
    )
    {
        string code;
        RandomRegionResult result = new();

        Province? province;
        do
        {
            code = GenerateRandomString(2);
            province = await provinces.Where(x => x.Code == code).FirstOrDefaultAsync();
        } while (province == null);
        result.Province = province;

        District? district;
        do
        {
            code = GenerateRandomString(3);
            district = await districts.Where(x => x.Code == code).FirstOrDefaultAsync();
        } while (district == null);
        result.District = district;

        Commune? commune;
        do
        {
            code = GenerateRandomString(5);
            commune = await communes.Where(x => x.Code == code).FirstOrDefaultAsync();
        } while (commune == null);
        result.Commune = commune;

        return result;
    }

    private static string GenerateRandomString(int length)
    {
        Random random = new();
        // Validate the input length
        if (length != 2 && length != 3 && length != 5)
        {
            throw new ArgumentException("Length must be 2, 3, or 5.");
        }

        // StringBuilder for efficient string concatenation
        StringBuilder builder = new(length);

        // Generate random digits
        for (int i = 0; i < length; i++)
        {
            int digit = random.Next(0, 10); // Random digit between 0 and 9
            builder.Append(digit);
        }

        return builder.ToString();
    }

    private static async Task<User[]> UserData(
        DbSet<Province> provinces,
        DbSet<District> districts,
        DbSet<Commune> communes
    )
    {
        string[] addresses =
        [
            "1823 Maple Avenue",
            "735 Park Lane",
            "4207 Oak Street",
            "5914 Elmwood Drive",
            "1068 Cedar Street",
            "2435 Willow Avenue",
            "3871 Birchwood Court",
            "1502 Walnut Street",
            "2704 Pine Street",
            "1328 Maplewood Avenue",
            "5172 Aspen Drive",
            "6809 Chestnut Lane",
            "4258 Magnolia Court",
            "9123 Oak Hill Road",
            "3810 Redwood Street",
            "7445 Pinecrest Avenue",
            "1680 Valley View Road",
            "2995 Brookside Drive",
            "5901 Country Lane",
            "4387 Maple Hill Drive",
            "1046 River Road",
            "8725 Hillcrest Drive",
        ];
        RandomRegionResult region = await RandomRegion(provinces, districts, communes);

        User user =
            new(
                "Chloe",
                "Kim",
                "chloe.kim",
                HashPassword(Credential.USER_DEFAULT_PASSWORD),
                "chloe.kim@gmail.com",
                "0925123123",
                new Address(
                    region.Province!,
                    region.District!,
                    region.Commune,
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1990, 10, 1),
                Status = UserStatus.Active,
            };

        RandomRegionResult johnDoeRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1985, 4, 23),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult aliceSmithRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1992, 7, 19),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult bobJohnsonRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1980, 3, 15),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult emilyBrownRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1995, 5, 5),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult jamesWilliamsRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1983, 11, 9),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult oliviaTaylorRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1998, 2, 18),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult danielLeeRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1987, 9, 21),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult sophiaGarciaRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1994, 12, 12),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult michaelMartinezRegion = await RandomRegion(
            provinces,
            districts,
            communes
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1978, 8, 8),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult isabellaHarrisRegion = await RandomRegion(
            provinces,
            districts,
            communes
        );
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1991, 1, 1),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult davidClarkRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1984, 6, 6),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult emmaRodriguezRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1993, 3, 3),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult andrewMooreRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1981, 10, 30),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult avaJacksonRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(2000, 4, 14),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult joshuaWhiteRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1986, 11, 17),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult charlotteThomasRegion = await RandomRegion(
            provinces,
            districts,
            communes
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1997, 7, 7),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult ethanKingRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1999, 9, 9),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult abigailScottRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1989, 2, 2),
                Status = UserStatus.Active,
                Gender = (Gender)new Random().Next(1, 3),
            };

        RandomRegionResult liamPerezRegion = await RandomRegion(provinces, districts, communes);
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
                    addresses[new Random().Next(0, addresses.Length)]
                )
            )
            {
                DayOfBirth = new DateTime(1988, 12, 25),
                Status = UserStatus.Inactive,
                Gender = (Gender)new Random().Next(1, 3),
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

internal class RandomRegionResult
{
    public Province? Province { get; set; }

    public District? District { get; set; }

    public Commune? Commune { get; set; }
}
