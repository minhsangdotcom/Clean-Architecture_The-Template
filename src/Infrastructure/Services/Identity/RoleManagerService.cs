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
    private readonly DbSet<Role> roleDbSet = context.Set<Role>();
    public DbSet<Role> Roles => roleDbSet;

    private readonly DbSet<RoleClaim> roleClaimDbSet = context.Set<RoleClaim>();
    public DbSet<RoleClaim> RoleClaims => roleClaimDbSet;

    private readonly DbSet<UserClaim> userClaimDbSet = context.Set<UserClaim>();

    private const string NotFoundMessage = $"{nameof(Role)} is not found";

    public async Task DeleteAsync(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        roleDbSet.Remove(role);
        await context.SaveChangesAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        await roleDbSet.AddAsync(role);
        await context.SaveChangesAsync();
        return role;
    }

    public async Task<IList<Role>> CreateRangeAsync(IEnumerable<Role> roles)
    {
        Guard.Against.NullOrEmpty(roles, nameof(roles));
        List<Role> createdRoles = [.. roles];
        await roleDbSet.AddRangeAsync(createdRoles);
        await context.SaveChangesAsync();
        return createdRoles;
    }

    public async Task<Role> UpdateAsync(Role role, IEnumerable<RoleClaim>? roleClaims)
    {
        Guard.Against.Null(role, nameof(role));
        await context.DatabaseFacade.BeginTransactionAsync();
        try
        {
            roleDbSet.Update(role);
            await context.SaveChangesAsync();

            if (roleClaims?.Any() == true)
            {
                await UpdateRoleClaimsAsync(roleClaims, role);
            }
            else
            {
                await ClearAllClaimsAsync(role.Id);
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
        await roleDbSet.FindAsync(id);

    public async Task<Role?> FindByIdAsync(Ulid id) =>
        await roleDbSet.Include(x => x.RoleClaims).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Role?> FindByNameAsync(string name) =>
        await roleDbSet
            .Include(x => x.RoleClaims)
            .FirstOrDefaultAsync(x => x.Name == name);

    public async Task<List<Role>> ListAsync() => await roleDbSet.ToListAsync();

    public async Task UpdateRoleClaimsAsync(IEnumerable<RoleClaim> roleClaims, Role role)
    {
        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await roleDbSet
                .Include(x => x.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .FirstOrDefaultAsync(x => x.Id == role.Id),
            NotFoundMessage
        );
        Guard.Against.Null(roleClaims, nameof(roleClaims), $"{nameof(roleClaims)} is not null");

        List<RoleClaim> rolesClaimsToProcess = [..roleClaims];
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;

        IEnumerable<RoleClaim> toAdd = rolesClaimsToProcess.ExceptBy(currentRoleClaims.Select(c => c.Id), c => c.Id);
        List<RoleClaim> toUpdate =
            [..currentRoleClaims.Where(c => rolesClaimsToProcess.Select(n => n.Id).Contains(c.Id))];
        IEnumerable<RoleClaim> toRemove =
            currentRoleClaims.ExceptBy(rolesClaimsToProcess.Select(n => n.Id), c => c.Id);

        IEnumerable<UserClaim> userClaims = ProcessUserClaimUpdate(
            toUpdate,
            rolesClaimsToProcess
        );

        // remove
        await RemoveClaimsFromRoleAsync(
            role,
            [
                .. toRemove.Select(x => new KeyValuePair<string, string>(
                    x.ClaimType,
                    x.ClaimValue
                )),
            ]
        );

        //update
        roleClaimDbSet.UpdateRange(toUpdate);
        userClaimDbSet.UpdateRange(userClaims);
        await context.SaveChangesAsync();

        var keyValuePairClaims = toAdd.Select(x => new KeyValuePair<string, string>(
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
            await roleDbSet
                .Include(x => x.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .Include(x => x.UserRoles)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == role.Id),
            NotFoundMessage
        );
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;
        List<KeyValuePair<string, string>> roleClaimsToProcess = [..claims];

        if (
            roleClaimsToProcess.Exists(x =>
                currentRoleClaims.Any(p => p.ClaimType == x.Key && p.ClaimValue == x.Value)
            )
        )
        {
            throw new Exception($"1 or more elements of {nameof(claims)} exists in role claims");
        }

        List<RoleClaim> newRoleClaims =
        [
            .. roleClaimsToProcess.Select(x => new RoleClaim
            {
                ClaimType = x.Key,
                ClaimValue = x.Value,
                RoleId = currentRole.Id,
            }),
        ];

        // update new user claims for users who are in role
        List<UserClaim> userClaims =
        [
            ..currentRole.UserRoles!
                .SelectMany(userRole => newRoleClaims.ConvertAll(roleClaim => new UserClaim()
                {
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue,
                    RoleClaimId = roleClaim.Id,
                    UserId = userRole.UserId,
                    Type = UserClaimType.Custom,
                }))
        ];

        if (newRoleClaims.Count > 0)
        {
            await roleClaimDbSet.AddRangeAsync(newRoleClaims);
            await context.SaveChangesAsync();
        }

        if (userClaims.Count > 0)
        {
            await userClaimDbSet.AddRangeAsync(userClaims);
            await context.SaveChangesAsync();
        }
    }

    public async Task RemoveClaimsFromRoleAsync(
        Role role,
        IEnumerable<KeyValuePair<string, string>> roleClaims
    )
    {
        List<KeyValuePair<string, string>> roleClaimsToProcess = roleClaims.ToList();
        if (roleClaimsToProcess.Count <= 0)
        {
            return;
        }

        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await FindByIdAsync(role.Id),
            NotFoundMessage
        );

        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims!;
        if (
            roleClaimsToProcess.Exists(x =>
                !currentRoleClaims.Any(p => p.ClaimType == x.Key && p.ClaimValue == x.Value)
            )
        )
        {
            throw new Exception("One or many claims is not existed in role.");
        }

        List<RoleClaim> claimsToDelete =
        [
            ..currentRoleClaims.Where(x =>
                roleClaimsToProcess.Exists(p => p.Key == x.ClaimType && p.Value == x.ClaimValue)
            )
        ];

        if (claimsToDelete.Count > 0)
        {
            roleClaimDbSet.RemoveRange(claimsToDelete);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IList<KeyValuePair<string, string>>> GetRolePermissionClaimsAsync() =>
        await RoleClaims
            .Where(claim => claim.ClaimType == ClaimTypes.Permission)
            .GroupBy(x => x.ClaimValue)
            .Select(x => new KeyValuePair<string, string>(ClaimTypes.Permission, x.Key))
            .ToListAsync();

    public Task<List<RoleClaim>> GetRoleClaimsAsync(Ulid roleId) => GetRoleClaimsAsync([roleId]);

    public async Task<List<RoleClaim>> GetRoleClaimsAsync(IEnumerable<Ulid> roleIds) =>
        await roleClaimDbSet.Where(x => roleIds.Contains(x.RoleId)).ToListAsync();

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, Ulid claimId) =>
        await roleDbSet.AnyAsync(x => x.Id == roleId && x.RoleClaims!.Any(p => p.Id == claimId));

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName) =>
        await roleDbSet.AnyAsync(x =>
            x.Id == roleId && x.RoleClaims!.Any(p => p.ClaimType == claimName)
        );

    public async Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName, string claimValue) =>
        await roleDbSet.AnyAsync(x =>
            x.Id == roleId
            && x.RoleClaims!.Any(p => p.ClaimType == claimName && p.ClaimValue == claimValue)
        );

    public async Task<bool> HasRoleClaimAsync(
        Ulid roleId,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        var roleClaims = await roleDbSet
            .Where(x => x.Id == roleId)
            .SelectMany(x => x.RoleClaims!)
            .ToListAsync();

        return roleClaims.Exists(x => claims.Any(p => p.Key == x.ClaimType && p.Value == x.ClaimValue));
    }

    private static IEnumerable<UserClaim> ProcessUserClaimUpdate(
        IEnumerable<RoleClaim> roleClaimsToModify,
        List<RoleClaim> rolesClaimsToProcess
    )
    {
        foreach (RoleClaim claim in roleClaimsToModify)
        {
            RoleClaim? correspondedClaim = rolesClaimsToProcess.Find(x =>
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

    private async Task ClearAllClaimsAsync(Ulid roleId)
    {
        var claims = await roleClaimDbSet.Where(c => c.RoleId == roleId).ToListAsync();
        if (claims.Count == 0) return;
        roleClaimDbSet.RemoveRange(claims);
        await context.SaveChangesAsync();
    }
}