using Application.Common.Interfaces.Services;
using Ardalis.GuardClauses;
using Domain.Aggregates.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class RoleManagerService(TheDbContext context) :
    IRoleManagerService
{
    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<RoleClaim> roleClaimContext = context.Set<RoleClaim>();
    public DbSet<RoleClaim> RoleClaims => roleClaimContext;

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
            await context.Database.BeginTransactionAsync();

            roleContext.Update(role);
            await context.SaveChangesAsync();

            if (roleClaims?.Any() == true)
            {
                await UpdateRoleClaimAsync(role, roleClaims);
            }

            await context.Database.CommitTransactionAsync();
            return role;
        }
        catch (Exception)
        {
            await context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Role> FindByIdAsync(Ulid id) =>
        Guard.Against.NotFound(
            $"{id}",
            await roleContext.Where(x => x.Id == id)
                .Include(x => x.RoleClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE);

    public async Task<Role> FindByNameAsync(string name) =>
        Guard.Against.NotFound(
            $"{name}",
            await roleContext.Where(x => x.Name == name)
                .Include(x => x.RoleClaims)
                .FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
        );

    public async Task<IEnumerable<Role>> ListAsync() => await roleContext.ToListAsync();

    public async Task UpdateRoleClaimAsync(Role role, IEnumerable<RoleClaim> roleClaims)
    {
        Role currentRole = await FindByIdAsync(role.Id);
        Guard.Against.Null(roleClaims, nameof(roleClaims), $"{nameof(roleClaims)} is not null");

        IEnumerable<RoleClaim> currentRoleClaims = currentRole.RoleClaims;
        IEnumerable<RoleClaim> shouldInserting = roleClaims.Where(x => !currentRoleClaims.Any(p => p.Id == x.Id));
        IEnumerable<RoleClaim> shouldModifying = currentRoleClaims.Where(x => roleClaims.Any(p => p.Id == x.Id));
        IEnumerable<RoleClaim> shouldRemoving = currentRoleClaims.Where(x => !roleClaims.Any(p => p.Id == x.Id));

        // remove
        await RemoveClaimsFromRoleAsync(role, shouldRemoving.Select(x => x.Id));

        foreach (var claim in shouldModifying)
        {
            var current = roleClaims.FirstOrDefault(x => x.Id == claim.Id);

            if (current == null)
            {
                continue;
            }

            claim.ClaimValue = current.ClaimValue;
        }

        roleClaimContext.UpdateRange(currentRoleClaims);
        await context.SaveChangesAsync();

        // insert
        await AddClaimsToRoleAsync(role, shouldInserting.ToDictionary(x => x.ClaimType, x => x.ClaimValue));
    }

    public async Task AddClaimsToRoleAsync(Role role, Dictionary<string, string> claims)
    {
        Role currentRole = await GetAsync(role.Id);

        IEnumerable<RoleClaim> roleClaims = claims.Select(x => new RoleClaim
        {
            ClaimType = x.Key,
            ClaimValue = x.Value,
            RoleId = currentRole.Id,
        });

        await roleClaimContext.AddRangeAsync(roleClaims);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<Ulid> claimIds)
    {
        if (!claimIds.Any())
        {
            return;
        }

        Role currentRole = await FindByIdAsync(role.Id);

        if (!claimIds.All(x => new HashSet<Ulid>(currentRole.RoleClaims.Select(p => p.Id)).Contains(x)))
        {
            throw new Exception($"{nameof(claimIds)} is not existed in role {nameof(role.Id)}.");
        }

        IEnumerable<RoleClaim> roleClaims = currentRole.RoleClaims.Where(x => claimIds.Contains(x.Id));

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
        await roleContext.AnyAsync(x => x.Id == roleId && x.RoleClaims.Any(p => p.ClaimType == claimName));

    private async Task<Role> GetAsync(Ulid id) =>
        Guard.Against.NotFound(
            $"{id}",
            await roleContext.Where(x => x.Id == id).FirstOrDefaultAsync(),
            NOT_FOUND_MESSAGE
    );
}