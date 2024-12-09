using System.Runtime.InteropServices;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Ardalis.GuardClauses;
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

    private readonly DbSet<UserClaim> userClaimContext = context.Set<UserClaim>();

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

    public async Task<IList<Role>> CreateRangeRoleAsync(IEnumerable<Role> roles)
    {
        await roleContext.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        return [.. roles];
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

        IEnumerable<RoleClaim> rolesClaimsToProcess = roleClaims;
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims;

        IEnumerable<RoleClaim> roleClaimsToInsert = rolesClaimsToProcess.Where(x =>
            !currentRoleClaims.Any(p => p.Id == x.Id)
        );
        List<RoleClaim> roleClaimsToModify =
        [
            .. currentRoleClaims.Where(x => rolesClaimsToProcess.Any(p => p.Id == x.Id)),
        ];
        IEnumerable<RoleClaim> roleClaimsToRemove = currentRoleClaims.Where(x =>
            !rolesClaimsToProcess.Any(p => p.Id == x.Id)
        );

        List<UserClaim> userClaims = ProcessUserClaimUpdate(
            ref roleClaimsToModify,
            rolesClaimsToProcess
        );

        // remove
        await RemoveClaimsFromRoleAsync(role, roleClaimsToRemove.Select(x => x.Id).ToArray());

        //update
        roleClaimContext.UpdateRange(roleClaimsToModify);
        userClaimContext.UpdateRange(userClaims);
        await context.SaveChangesAsync();

        // insert
        await AddClaimsToRoleAsync(
            role,
            roleClaimsToInsert.Select(x => new KeyValuePair<string, string>(
                x.ClaimType,
                x.ClaimValue
            ))
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
        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims;
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
        ICollection<UserRole> users = currentRole.UserRoles;
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
                    Type = KindaUserClaimType.Custom,
                }
            );
            userClaims.AddRange(additionUserClaims);
        }

        await roleClaimContext.AddRangeAsync(roleClaimsToInsert);
        await context.SaveChangesAsync();
        await userClaimContext.AddRangeAsync(userClaims);
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

        ICollection<RoleClaim> currentRoleClaims = currentRole.RoleClaims;
        if (claimIds.Any(x => !currentRoleClaims.Any(p => p.Id == x)))
        {
            throw new Exception($"{nameof(claimIds)} is not existed in role {nameof(role.Id)}.");
        }

        IEnumerable<RoleClaim> roleClaims = currentRoleClaims.Where(x => claimIds.Contains(x.Id));

        roleClaimContext.RemoveRange(roleClaims);
        await context.SaveChangesAsync();
    }

    public Task<List<RoleClaim>> GetClaimsByRoleAsync(Ulid roleId) =>
        GetClaimsByRolesAsync([roleId]);

    public async Task<List<RoleClaim>> GetClaimsByRolesAsync(IEnumerable<Ulid> roleIds) =>
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

    private static List<UserClaim> ProcessUserClaimUpdate(
        ref List<RoleClaim> roleClaimsToModify,
        IEnumerable<RoleClaim> rolesClaimsToProcess
    )
    {
        List<UserClaim> userClaims = [];
        Span<RoleClaim> spans = CollectionsMarshal.AsSpan(roleClaimsToModify);
        for (int i = 0; i < spans.Length; i++)
        {
            RoleClaim claim = spans[i];
            RoleClaim? correspondedClaim = rolesClaimsToProcess.FirstOrDefault(x =>
                x.Id == claim.Id
            );

            if (correspondedClaim == null)
            {
                continue;
            }

            claim.ClaimValue = correspondedClaim.ClaimValue;
            List<UserClaim> updatedUserClaims = claim.UpdateUserClaim();
            userClaims.AddRange(updatedUserClaims);
        }

        return userClaims;
    }
}
