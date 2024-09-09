using Application.Common.Interfaces.Services;
using Ardalis.GuardClauses;
using Domain.Aggregates.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

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

    public async Task<Role> UpdateRoleAsync(Role role, IEnumerable<RoleClaim>? roleClaims)
    {
        try
        {
            await context.DatabaseFacade.BeginTransactionAsync();

            roleContext.Update(role);
            await context.SaveChangesAsync();

            if (roleClaims?.Any() == true)
            {
                await UpdateRoleClaimAsync(role, roleClaims);
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

    public async Task<Role?> FindByIdAsync(Ulid id) =>
        await roleContext.Where(x => x.Id == id).Include(x => x.RoleClaims).FirstOrDefaultAsync();

    public async Task<Role?> FindByNameAsync(string name) =>
        await roleContext
            .Where(x => x.Name == name)
            .Include(x => x.RoleClaims)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Role>> ListAsync() => await roleContext.ToListAsync();

    public async Task UpdateRoleClaimAsync(Role role, IEnumerable<RoleClaim> roleClaims)
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

        IEnumerable<RoleClaim> currentRoleClaims = currentRole.RoleClaims;
        IEnumerable<RoleClaim> shouldInserting = roleClaims.Where(x =>
            !currentRoleClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<RoleClaim> shouldModifying = currentRoleClaims.Where(x =>
            roleClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<RoleClaim> shouldRemoving = currentRoleClaims.Where(x =>
            !roleClaims.Any(p => p.Id == x.Id)
        );

        foreach (var claim in shouldModifying)
        {
            var current = roleClaims.FirstOrDefault(x => x.Id == claim.Id);

            if (current == null)
            {
                continue;
            }

            claim.ClaimValue = current.ClaimValue;
            claim.UpdateUserClaim();
        }

        // remove
        await RemoveClaimsFromRoleAsync(role, shouldRemoving.Select(x => x.Id));

        //update
        roleClaimContext.UpdateRange(currentRoleClaims);
        UserClaimsContext.UpdateRange(currentRoleClaims.SelectMany(x => x.UserClaims!));
        await context.SaveChangesAsync();

        // insert
        await AddClaimsToRoleAsync(
            role,
            shouldInserting.Select(x => new KeyValuePair<string, string>(x.ClaimType, x.ClaimValue))
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
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x!.UserClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );

        IEnumerable<RoleClaim> roleClaims = claims.Select(x => new RoleClaim
        {
            ClaimType = x.Key,
            ClaimValue = x.Value,
            RoleId = currentRole.Id,
        });

        List<User> users = currentRole.UserRoles.Select(x => x.User!).ToList();

        users.ForEach(user =>
            user!.AddUserClaim(
                roleClaims.Select(roleClaim => new UserClaim()
                {
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue,
                    RoleClaimId = roleClaim.Id,
                })
            )
        );

        await roleClaimContext.AddRangeAsync(roleClaims);
        await context.SaveChangesAsync();
        context.Set<User>().UpdateRange(users);
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
            await roleContext
                .Where(x => x.Id == role.Id)
                .Include(x => x.RoleClaims)
                .ThenInclude(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );

        if (
            !claimIds.All(x =>
                new HashSet<Ulid>(currentRole.RoleClaims.Select(p => p.Id)).Contains(x)
            )
        )
        {
            throw new Exception($"{nameof(claimIds)} is not existed in role {nameof(role.Id)}.");
        }

        IEnumerable<RoleClaim> roleClaims = currentRole.RoleClaims.Where(x =>
            claimIds.Contains(x.Id)
        );
        IEnumerable<UserClaim> userClaims = roleClaims.SelectMany(x => x.UserClaims!);

        roleClaimContext.RemoveRange(roleClaims);
        UserClaimsContext.RemoveRange(userClaims);
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
        Guard.Against.NotFound(
            $"{id}",
            await roleContext.Where(x => x.Id == id).FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );
}
