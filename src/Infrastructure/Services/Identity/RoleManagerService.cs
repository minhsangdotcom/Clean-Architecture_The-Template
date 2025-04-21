using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Ardalis.GuardClauses;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constants;

namespace Infrastructure.Services.Identity;

public class RoleManagerService(IDbContext context) : IRoleManagerService
{
    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<RoleClaim> roleClaimContext = context.Set<RoleClaim>();
    public DbSet<RoleClaim> RoleClaims => roleClaimContext;

    private readonly DbSet<UserClaim> userClaimContext = context.Set<UserClaim>();

    private const string NOT_FOUND_MESSAGE = $"{nameof(Role)} is not found";

    public async Task DeleteAsync(Role role)
    {
        roleContext.Remove(role);
        await context.SaveChangesAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        await roleContext.AddAsync(role);
        await context.SaveChangesAsync();

        return role;
    }

    public async Task<IList<Role>> CreateRangeAsync(IEnumerable<Role> roles)
    {
        await roleContext.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        return [.. roles];
    }

    public async Task<Role> UpdateAsync(Role role, IEnumerable<RoleClaim>? roleClaims)
    {
        try
        {
            await context.DatabaseFacade.BeginTransactionAsync();

            roleContext.Update(role);
            await context.SaveChangesAsync();

            if (roleClaims?.Any() == true)
            {
                await UpdateRoleClaimsAsync(roleClaims, role);
            }
            else
            {
                List<RoleClaim> claimsToDelete = await roleClaimContext
                    .Where(x => x.RoleId == role.Id)
                    .ToListAsync();
                roleClaimContext.RemoveRange(claimsToDelete);
                await context.SaveChangesAsync();
            }

            await context.DatabaseFacade.CommitTransactionAsync();
            return role;
        }
        catch (Exception)
        {
            await context.DatabaseFacade.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Role?> GetByIdAsync(Ulid id) =>
        await roleContext.Where(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Role?> FindByIdAsync(Ulid id) =>
        await roleContext.Where(x => x.Id == id).Include(x => x.RoleClaims).FirstOrDefaultAsync();

    public async Task<Role?> FindByNameAsync(string name) =>
        await roleContext
            .Where(x => x.Name == name)
            .Include(x => x.RoleClaims)
            .FirstOrDefaultAsync();

    public async Task<List<Role>> ListAsync() => await roleContext.ToListAsync();

    public async Task UpdateRoleClaimsAsync(IEnumerable<RoleClaim> roleClaims, Role role)
    {
        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await roleContext
                .Where(x => x.Id == role.Id)
                .Include(x => x.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );
        Guard.Against.Null(roleClaims, nameof(roleClaims), $"{nameof(roleClaims)} is not null");

        IEnumerable<RoleClaim> rolesClaimsToProcess = roleClaims;
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;

        IEnumerable<RoleClaim> roleClaimsToInsert = rolesClaimsToProcess.Where(x =>
            !currentRoleClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<RoleClaim> roleClaimsToModify = currentRoleClaims.Where(x =>
            rolesClaimsToProcess.Any(p => p.Id == x.Id)
        );
        IEnumerable<RoleClaim> roleClaimsToRemove = currentRoleClaims.Where(x =>
            !rolesClaimsToProcess.Any(p => p.Id == x.Id)
        );

        IEnumerable<UserClaim> userClaims = ProcessUserClaimUpdate(
            roleClaimsToModify,
            rolesClaimsToProcess
        );

        // remove
        await RemoveClaimsFromRoleAsync(
            role,
            [
                .. roleClaimsToRemove.Select(x => new KeyValuePair<string, string>(
                    x.ClaimType,
                    x.ClaimValue
                )),
            ]
        );

        //update
        roleClaimContext.UpdateRange(roleClaimsToModify);
        userClaimContext.UpdateRange(userClaims);
        await context.SaveChangesAsync();

        var keyValuePairClaims = roleClaimsToInsert.Select(x => new KeyValuePair<string, string>(
            x.ClaimType,
            x.ClaimValue
        ));
        // insert
        await AssignClaimsToRoleAsync(role, keyValuePairClaims);
    }

    public async Task AssignClaimsToRoleAsync(
        Role role,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await roleContext
                .Where(x => x.Id == role.Id)
                .Include(x => x.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .Include(x => x.UserRoles)
                .AsSplitQuery()
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;
        IEnumerable<KeyValuePair<string, string>> roleClaimsToProcess = claims;

        if (
            roleClaimsToProcess.Any(x =>
                currentRoleClaims.Any(p => p.ClaimType == x.Key && p.ClaimValue == x.Value)
            )
        )
        {
            throw new Exception($"1 or more elements of {nameof(claims)} exists in role claims");
        }

        List<RoleClaim> roleClaimsToInsert =
        [
            .. roleClaimsToProcess.Select(x => new RoleClaim
            {
                ClaimType = x.Key,
                ClaimValue = x.Value,
                RoleId = currentRole.Id,
            }),
        ];

        // update new user claims for users who are in role
        ICollection<UserRole> users = currentRole.UserRoles!;
        List<UserClaim> userClaims = [];
        foreach (UserRole user in users)
        {
            Ulid userId = user.UserId;
            IEnumerable<UserClaim> additionUserClaims = roleClaimsToInsert.Select(
                roleClaim => new UserClaim()
                {
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue,
                    RoleClaimId = roleClaim.Id,
                    UserId = userId,
                    Type = UserClaimType.Custom,
                }
            );
            userClaims.AddRange(additionUserClaims);
        }

        await roleClaimContext.AddRangeAsync(roleClaimsToInsert);
        await context.SaveChangesAsync();
        await userClaimContext.AddRangeAsync(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromRoleAsync(
        Role role,
        IEnumerable<KeyValuePair<string, string>> roleClaims
    )
    {
        if (!roleClaims.Any())
        {
            return;
        }

        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await FindByIdAsync(role.Id),
            NOT_FOUND_MESSAGE
        );

        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;
        if (
            roleClaims.Any(x =>
                !currentRoleClaims.Any(p => p.ClaimType == x.Key && p.ClaimValue == x.Value)
            )
        )
        {
            throw new Exception("One or many claims is not existed in role.");
        }

        IEnumerable<RoleClaim> claimsToDelete = currentRoleClaims.Where(x =>
            roleClaims.Any(p => p.Key == x.ClaimType && p.Value == x.ClaimValue)
        );

        roleClaimContext.RemoveRange(claimsToDelete);
        await context.SaveChangesAsync();
    }

    public async Task<IList<RoleClaim>> GetRolePermissionClaimsAsync() =>
        await RoleClaims.Where(claim => claim.ClaimType == ClaimTypes.Permission).ToListAsync();

    public Task<List<RoleClaim>> GetRoleClaimsAsync(Ulid roleId) => GetRoleClaimsAsync([roleId]);

    public async Task<List<RoleClaim>> GetRoleClaimsAsync(IEnumerable<Ulid> roleIds) =>
        await roleClaimContext.Where(x => roleIds.Contains(x.RoleId)).ToListAsync();

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, Ulid claimId) =>
        await roleContext.AnyAsync(x => x.Id == roleId && x.RoleClaims!.Any(p => p.Id == claimId));

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName) =>
        await roleContext.AnyAsync(x =>
            x.Id == roleId && x.RoleClaims!.Any(p => p.ClaimType == claimName)
        );

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName, string claimValue) =>
        await roleContext.AnyAsync(x =>
            x.Id == roleId
            && x.RoleClaims!.Any(p => p.ClaimType == claimName && p.ClaimValue == claimValue)
        );

    public async Task<bool> HasRoleClaimAsync(
        Ulid roleId,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        var roleClaims = await roleContext
            .Where(x => x.Id == roleId)
            .SelectMany(x => x.RoleClaims!)
            .ToListAsync();

        return roleClaims.Any(x => claims.Contains(new(x.ClaimType, x.ClaimValue)));
    }

    private static IEnumerable<UserClaim> ProcessUserClaimUpdate(
        IEnumerable<RoleClaim> roleClaimsToModify,
        IEnumerable<RoleClaim> rolesClaimsToProcess
    )
    {
        foreach (RoleClaim claim in roleClaimsToModify)
        {
            RoleClaim? correspondedClaim = rolesClaimsToProcess.FirstOrDefault(x =>
                x.Id == claim.Id
            );

            if (correspondedClaim == null)
            {
                continue;
            }

            claim.ClaimValue = correspondedClaim.ClaimValue;
            List<UserClaim> updatedUserClaims = claim.UpdateUserClaim();

            for (int i = 0; i < updatedUserClaims.Count; i++)
            {
                yield return updatedUserClaims[i];
            }
        }
    }
}
