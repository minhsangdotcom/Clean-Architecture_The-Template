using System.Data;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Ardalis.GuardClauses;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constants;

namespace Infrastructure.Services.Identity;

public class UserManagerService(IRoleManagerService roleManagerService, IDbContext context)
    : IUserManagerService
{
    private readonly DbSet<UserRole> userRoleContext = context.Set<UserRole>();
    public DbSet<UserRole> UserRoles => userRoleContext;

    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<UserClaim> userClaimsContext = context.Set<UserClaim>();
    public DbSet<UserClaim> UserClaims => userClaimsContext;

    private readonly DbSet<User> userContext = context.Set<User>();

    public async Task CreateAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaim>? claims = null
    )
    {
        await AssignRolesToUserAsync(user, roleIds);
        await AssignClaimsToUserAsync(user, claims ?? []);
    }

    public async Task UpdateAsync(
        User user,
        IEnumerable<Ulid>? roleIds,
        IEnumerable<UserClaim>? claims = null
    )
    {
        // update role for user
        await UpdateUserRolesAsync(user, roleIds);

        // update custom user claim
        await UpdateUserClaimsAsync(user, claims);
    }

    public async Task AssignRolesToUserAsync(User user, IEnumerable<Ulid> roleIds)
    {
        if (!roleIds.Any())
        {
            return;
        }

        User? currentUser = await userContext
            .Where(x => x.Id == user.Id)
            .Include(x => x.UserRoles)
            .Include(x => x.UserClaims)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        Guard.Against.NotFound($"{user.Id}", currentUser, nameof(user));
        IEnumerable<Ulid> rolesToProcess = roleIds;

        if (
            await roleContext.CountAsync(x => rolesToProcess.Contains(x.Id))
            != rolesToProcess.Count()
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is not found", nameof(roleIds));
        }

        if (
            await userRoleContext.AnyAsync(x =>
                x.UserId == user.Id && rolesToProcess.Contains(x.RoleId)
            )
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is existence in user", nameof(roleIds));
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;
        List<Ulid> rolesToInsert =
        [
            .. rolesToProcess.Where(x => !currentUserRoles.Any(p => p.RoleId == x)),
        ];

        await userRoleContext.AddRangeAsync(
            rolesToInsert.Select(x => new UserRole { RoleId = x, UserId = currentUser.Id })
        );
        await context.SaveChangesAsync();

        //derive all role claims for users if user is assigned specific role.
        List<RoleClaim> roleClaims = await roleManagerService.GetRoleClaimsAsync(rolesToInsert);
        IEnumerable<UserClaim> userClaimsToInsert = roleClaims.Select(x => new UserClaim
        {
            UserId = user.Id,
            ClaimType = x.ClaimType,
            ClaimValue = x.ClaimValue,
            RoleClaimId = x.Id,
            Type = UserClaimType.Custom,
        });
        ICollection<UserClaim> currentUserClaims = currentUser.UserClaims!;

        IEnumerable<UserClaim> uniqueUserClaimsToInsert = userClaimsToInsert.Where(x =>
            !currentUserClaims.Any(p => p.ClaimType == x.ClaimType && p.ClaimValue == x.ClaimValue)
        );

        await userClaimsContext.AddRangeAsync(uniqueUserClaimsToInsert);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserRolesAsync(User user, IEnumerable<Ulid>? roleIds)
    {
        if (roleIds == null || !roleIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<Ulid> rolesToProcess = roleIds;

        if (
            await roleContext.CountAsync(x => rolesToProcess.Contains(x.Id))
            != rolesToProcess.Count()
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;

        IEnumerable<Ulid> rolesToRemove = currentUserRoles
            .Where(x => !rolesToProcess.Contains(x.RoleId))
            .Select(x => x.RoleId);
        IEnumerable<Ulid> rolesToInsert = rolesToProcess.Where(x =>
            !currentUserRoles.Any(p => p.RoleId == x)
        );

        await RemoveRolesFromUserAsync(currentUser, rolesToRemove);
        await AssignRolesToUserAsync(currentUser, rolesToInsert);
    }

    public async Task RemoveRolesFromUserAsync(User user, IEnumerable<Ulid> roleIds)
    {
        if (!roleIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserRoles)!
                .ThenInclude(x => x.Role)!
                .ThenInclude(x => x!.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .AsSplitQuery()
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<Ulid> rolesToProcess = roleIds;
        if (
            await roleContext.CountAsync(x => rolesToProcess.Contains(x.Id))
            != rolesToProcess.Count()
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;
        if (rolesToProcess.Any(x => !currentUserRoles.Any(p => p.RoleId == x)))
        {
            throw new ArgumentException(
                $"{nameof(roleIds)} is not existed in user {nameof(user.Id)}"
            );
        }

        IList<UserRole> userRoles =
        [
            .. currentUserRoles.Where(x => rolesToProcess.Contains(x.RoleId)),
        ];

        IEnumerable<UserClaim> userClaims = userRoles
            .Select(x => x.Role)
            .SelectMany(x => x!.RoleClaims!)
            .SelectMany(x => x.UserClaims!);

        userRoleContext.RemoveRange(userRoles);
        await context.SaveChangesAsync();
        userClaimsContext.RemoveRange(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task AssignClaimsToUserAsync(User user, IEnumerable<UserClaim> claims)
    {
        if (claims == null || !claims.Any())
        {
            return;
        }
        _ = await GetUserAsync(user.Id);

        await userClaimsContext.AddRangeAsync(claims);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDefaultUserClaimsAsync(IEnumerable<UserClaim> defaultUserClaims)
    {
        userClaimsContext.UpdateRange(defaultUserClaims);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserClaimsAsync(User user, IEnumerable<UserClaim>? claims)
    {
        if (claims == null || !claims.Any())
        {
            return;
        }

        User? currentUser = Guard.Against.NotFound(
            $"{user}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> customUserClaims = currentUser.UserClaims!.Where(x =>
            x.Type == UserClaimType.Custom
        );
        IEnumerable<UserClaim> claimsToProcess = claims;

        IEnumerable<UserClaim> claimsToInsert = claimsToProcess.Where(x =>
            !customUserClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<UserClaim> claimsToUpdate = customUserClaims.Where(x =>
            claimsToProcess.Any(p => p.Id == x.Id)
        );

        IEnumerable<UserClaim> claimsToRemove = customUserClaims.Where(x =>
            x.RoleClaimId == null && !claimsToProcess.Any(p => p.Id == x.Id)
        );

        ProcessUserClaimUpdate(claimsToUpdate, claimsToProcess);

        userClaimsContext.RemoveRange(claimsToRemove);
        userClaimsContext.UpdateRange(claimsToUpdate);
        await userClaimsContext.AddRangeAsync(claimsToInsert);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromUserAsync(User user, IEnumerable<Ulid> claimIds)
    {
        if (!claimIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> currentUserClaims = currentUser.UserClaims!.Where(p =>
            p.Type == UserClaimType.Custom
        );
        if (claimIds.Any(x => !currentUserClaims.Any(p => p.Id == x)))
        {
            throw new ArgumentException(
                $"{nameof(claimIds)} is not existed in user {nameof(user.Id)}."
            );
        }

        IEnumerable<UserClaim> claimsToRemove = currentUserClaims.Where(x =>
            claimIds.Contains(x.Id)
        );

        userClaimsContext.RemoveRange(claimsToRemove);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromUserAsync(User user, IEnumerable<UserClaim> claims)
    {
        IEnumerable<Ulid> userClaims = claims.Select(x => x.Id);
        await RemoveClaimsFromUserAsync(user, userClaims);
    }

    public async Task<List<Role>> GetUserRoles(Ulid userId) =>
        await userRoleContext.Where(x => x.UserId == userId).Select(x => x.Role!).ToListAsync();

    public async Task<List<UserClaim>> GetUserClaims(Ulid userId) =>
        await userClaimsContext.Where(x => x.UserId == userId).ToListAsync();

    public async Task<bool> HasUserRolesAsync(Ulid id, IEnumerable<string> roleNames) =>
        await userContext.AnyAsync(x =>
            x.Id == id && x.UserRoles!.Any(p => roleNames.Contains(p.Role!.Name))
        );

    public async Task<bool> HasUserClaimsAsync(
        Ulid id,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        List<UserClaim> userClaims = await UserClaims
            .Where(x => x.UserId == id && x.ClaimType == ClaimTypes.Permission)
            .ToListAsync();

        return userClaims.Exists(x => claims.Contains(new(x.ClaimType, x.ClaimValue)));
    }

    //! benchmark
    public async Task<bool> HasUserPermissionAsync(Ulid id, IEnumerable<string> claims)
    {
        List<string> permissions = await UserClaims
            .Where(x => x.UserId == id && x.ClaimType == ClaimTypes.Permission)
            .Select(x => x.ClaimValue)
            .ToListAsync();

        var claimsSet = new HashSet<string>(claims);
        if (permissions.Any(claimsSet.Contains))
        {
            return true;
        }

        var permissionsSet = new HashSet<string>(permissions);
        return Credential
            .permissions[typeof(Permission).FullName!]
            .Any(x => permissionsSet.Contains(x.Key) && x.Value.Exists(p => claimsSet.Contains(p)));
    }

    public async Task<bool> HasUserClaimsAndRolesAsync(
        Ulid id,
        IEnumerable<string> roles,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        bool isHaRole = await HasUserRolesAsync(id, roles);
        bool isHasClaim = await HasUserClaimsAsync(id, claims);

        return isHaRole && isHasClaim;
    }

    private async Task<User> GetUserAsync(Ulid id) =>
        Guard.Against.NotFound(
            $"{id}",
            await userContext.Where(x => x.Id == id).FirstOrDefaultAsync(),
            nameof(id)
        );

    private static void ProcessUserClaimUpdate(
        IEnumerable<UserClaim> claimsToUpdate,
        IEnumerable<UserClaim> claimsToProcess
    )
    {
        foreach (UserClaim claim in claimsToUpdate)
        {
            var correspondenceClaim = claimsToProcess.FirstOrDefault(x => x.Id == claim.Id);

            if (correspondenceClaim == null)
            {
                continue;
            }
            claim.ClaimValue = correspondenceClaim.ClaimValue!;
        }
    }
}
