using System.Runtime.InteropServices;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Ardalis.GuardClauses;
using Dapper;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class RoleManagerService(IDbContext context) : IRoleManagerService
{
    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<RoleClaim> roleClaimContext = context.Set<RoleClaim>();
    public DbSet<RoleClaim> RoleClaims => roleClaimContext;

    public DbSet<UserClaim> UserClaimsContext = context.Set<UserClaim>();

    private const string NOT_FOUND_MESSAGE = $"{nameof(Role)} is not found";

    public async Task DeleteRoleAsync(Role role)
    {
        roleContext.Remove(role);
        await context.SaveChangesAsync();
    }

    public async Task<Role> CreateRoleAsync(Role role)
    {
        await roleContext.AddAsync(role);
        await context.SaveChangesAsync();

        return role;
    }

    public async Task<IEnumerable<Role>> CreateRangeRoleAsync(IEnumerable<Role> roles)
    {
        await roleContext.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        return roles;
    }

    public async Task<Role> UpdateRoleAsync(Role role, IEnumerable<RoleClaim>? roleClaims)
    {
        try
        {
            await context.DatabaseFacade.BeginTransactionAsync();

            roleContext.Update(role);
            await context.SaveChangesAsync();

            if (roleClaims?.Any() == true)
            {
                await UpdateRoleClaimAsync(roleClaims, role);
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

    public async Task<IEnumerable<Role>> ListAsync() => await roleContext.ToListAsync();

    public async Task UpdateRoleClaimAsync(IEnumerable<RoleClaim> roleClaims, Role role)
    {
        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await roleContext
                .Where(x => x.Id == role.Id)
                .Include(x => x.RoleClaims)
                .ThenInclude(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );

        Guard.Against.Null(roleClaims, nameof(roleClaims), $"{nameof(roleClaims)} is not null");
        List<RoleClaim> listRoleClaim = roleClaims.AsList();

        List<RoleClaim> currentRoleClaims = currentRole.RoleClaims.AsList();
        IEnumerable<RoleClaim> shouldInsert = listRoleClaim.FindAll(x =>
            !currentRoleClaims.Exists(p => p.Id == x.Id)
        );
        List<RoleClaim> shouldModify = currentRoleClaims.FindAll(x =>
            listRoleClaim.Exists(p => p.Id == x.Id)
        );
        IEnumerable<RoleClaim> shouldRemove = currentRoleClaims.FindAll(x =>
            !listRoleClaim.Exists(p => p.Id == x.Id)
        );

        List<UserClaim> userClaims = [];
        for (int i = 0; i < shouldModify.Count; i++)
        {
            RoleClaim claim = shouldModify[i];
            RoleClaim? correspondedClaim = listRoleClaim.Find(x => x.Id == claim.Id);

            if (correspondedClaim == null)
            {
                continue;
            }

            claim.ClaimValue = correspondedClaim.ClaimValue;
            List<UserClaim> updatedUserClaims = claim.UpdateUserClaim();
            userClaims.AddRange(updatedUserClaims);
        }

        // remove
        await RemoveClaimsFromRoleAsync(role, shouldRemove.Select(x => x.Id));

        //update
        roleClaimContext.UpdateRange(shouldModify);
        UserClaimsContext.UpdateRange(userClaims);
        await context.SaveChangesAsync();

        // insert
        await AddClaimsToRoleAsync(
            role,
            shouldInsert.Select(x => new KeyValuePair<string, string>(x.ClaimType, x.ClaimValue))
        );
    }

    public async Task AddClaimsToRoleAsync(
        Role role,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await roleContext
                .Where(x => x.Id == role.Id)
                .Include(x => x.RoleClaims)
                .ThenInclude(x => x.UserClaims)
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );

        if (
            claims.Any(x =>
                currentRole.RoleClaims.Any(p => p.ClaimType == x.Key && p.ClaimValue == x.Value)
            )
        )
        {
            throw new Exception($"1 or more elements of {nameof(claims)} exists in role claims");
        }

        List<RoleClaim> roleClaims = claims
            .Select(x => new RoleClaim
            {
                ClaimType = x.Key,
                ClaimValue = x.Value,
                RoleId = currentRole.Id,
            })
            .ToList();

        // update new user claims for users who are in role
        ICollection<UserRole> users = currentRole.UserRoles;
        List<UserClaim> userClaims = [];
        foreach (UserRole user in users)
        {
            Ulid userId = user.UserId;
            IEnumerable<UserClaim> additionUserClaims = roleClaims.Select(
                roleClaim => new UserClaim()
                {
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue,
                    RoleClaimId = roleClaim.Id,
                    UserId = userId,
                    Type = KindaUserClaimType.Custom,
                }
            );
            userClaims.AddRange(additionUserClaims);
        }

        await roleClaimContext.AddRangeAsync(roleClaims);
        await context.SaveChangesAsync();
        await UserClaimsContext.AddRangeAsync(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<Ulid> claimIds)
    {
        if (!claimIds.Any())
        {
            return;
        }

        Role currentRole = Guard.Against.NotFound(
            $"{role.Id}",
            await FindByIdAsync(role.Id),
            NOT_FOUND_MESSAGE
        );

        List<RoleClaim> currentRoleClaims = currentRole.RoleClaims.AsList();
        if (claimIds.Any(x => !currentRoleClaims.Exists(p => p.Id == x)))
        {
            throw new Exception($"{nameof(claimIds)} is not existed in role {nameof(role.Id)}.");
        }

        IEnumerable<RoleClaim> roleClaims = currentRoleClaims.FindAll(x => claimIds.Contains(x.Id));

        roleClaimContext.RemoveRange(roleClaims);
        await context.SaveChangesAsync();
    }

    public Task<IEnumerable<RoleClaim>> GetClaimsByRoleAsync(Ulid roleId) =>
        GetClaimsByRolesAsync([roleId]);

    public async Task<IEnumerable<RoleClaim>> GetClaimsByRolesAsync(IEnumerable<Ulid> roleIds) =>
        await roleClaimContext.Where(x => roleIds.Contains(x.RoleId)).ToListAsync();

    public async Task<bool> HasClaimInRoleAsync(Ulid roleId, Ulid claimId) =>
        await roleContext.AnyAsync(x => x.Id == roleId && x.RoleClaims.Any(p => p.Id == claimId));

    public async Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName) =>
        await roleContext.AnyAsync(x =>
            x.Id == roleId && x.RoleClaims.Any(p => p.ClaimType == claimName)
        );

    public async Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName, string claimValue) =>
        await roleContext.AnyAsync(x =>
            x.Id == roleId
            && x.RoleClaims.Any(p => p.ClaimType == claimName && p.ClaimValue == claimValue)
        );

    public async Task<bool> HasClaimInRoleAsync(
        Ulid roleId,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        var roleClaims = await roleContext
            .Where(x => x.Id == roleId)
            .SelectMany(x => x.RoleClaims)
            .ToListAsync();

        return roleClaims.Any(x => claims.Contains(new(x.ClaimType, x.ClaimValue)));
    }

    private async Task<Role> GetAsync(Ulid id) =>
        Guard.Against.NotFound($"{id}", await GetByIdAsync(id), NOT_FOUND_MESSAGE);
}
