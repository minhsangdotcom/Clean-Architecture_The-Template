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
    private readonly DbSet<UserRole> userRoleDbSet = context.Set<UserRole>();
    public DbSet<UserRole> UserRoles => userRoleDbSet;

    private readonly DbSet<Role> roleDbSet = context.Set<Role>();
    public DbSet<Role> Roles => roleDbSet;

    private readonly DbSet<UserClaim> userClaimsDbSet = context.Set<UserClaim>();
    public DbSet<UserClaim> UserClaims => userClaimsDbSet;

    private readonly DbSet<User> userDbSet = context.Set<User>();
    public DbSet<User> User => userDbSet;


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
        List<Ulid> rolesToProcess = roleIds.ToList();
        if (rolesToProcess.Count == 0)
        {
            return;
        }

        User? currentUser = await userDbSet
            .Include(x => x.UserRoles)
            .Include(x => x.UserClaims)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == user.Id);

        Guard.Against.NotFound($"{user.Id}", currentUser, nameof(user));

        if (
            await roleDbSet.CountAsync(x => rolesToProcess.Contains(x.Id))
            != rolesToProcess.Count
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is not found", nameof(roleIds));
        }

        if (
            await userRoleDbSet.AnyAsync(x =>
                x.UserId == user.Id && rolesToProcess.Contains(x.RoleId)
            )
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is existence in user", nameof(roleIds));
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;
        List<Ulid> toInsert = [..rolesToProcess.Except(currentUserRoles.Select(ur => ur.RoleId))];
        await userRoleDbSet.AddRangeAsync(
            toInsert.Select(x => new UserRole { RoleId = x, UserId = currentUser.Id })
        );
        await context.SaveChangesAsync();


        //derive all role claims for users if user is assigned specific role.
        List<RoleClaim> roleClaims = await roleManagerService.GetRoleClaimsAsync(toInsert);
        List<UserClaim> userClaimsToInsert =
        [
            ..roleClaims
                .Where(x =>
                    !currentUser.UserClaims!.Any(p => p.ClaimType == x.ClaimType && p.ClaimValue == x.ClaimValue)
                )
                .Select(x => new UserClaim
                {
                    UserId = user.Id,
                    ClaimType = x.ClaimType,
                    ClaimValue = x.ClaimValue,
                    RoleClaimId = x.Id,
                    Type = UserClaimType.Custom,
                })
        ];
        if (userClaimsToInsert.Count > 0)
        {
            await userClaimsDbSet.AddRangeAsync(userClaimsToInsert);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateUserRolesAsync(User user, IEnumerable<Ulid>? roleIds)
    {
        List<Ulid>? rolesToProcess = roleIds?.ToList();
        if (rolesToProcess?.Count <= 0)
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userDbSet
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.Id == user.Id),
            nameof(user)
        );

        if (
            await roleDbSet.CountAsync(x => rolesToProcess!.Contains(x.Id))
            != rolesToProcess!.Count
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;

        IEnumerable<Ulid> rolesToRemove = currentUserRoles
            .Where(x => !rolesToProcess.Contains(x.RoleId))
            .Select(x => x.RoleId);
        List<Ulid> rolesToInsert = rolesToProcess.FindAll(x => currentUserRoles.All(p => p.RoleId != x));

        await RemoveRolesFromUserAsync(currentUser, rolesToRemove);
        await AssignRolesToUserAsync(currentUser, rolesToInsert);
    }

    public async Task RemoveRolesFromUserAsync(User user, IEnumerable<Ulid> roleIds)
    {
        List<Ulid> rolesToProcess = roleIds.ToList();
        if (rolesToProcess.Count <= 0)
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userDbSet
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserRoles)!
                .ThenInclude(x => x.Role)!
                .ThenInclude(x => x!.RoleClaims)!
                .ThenInclude(x => x.UserClaims)
                .AsSplitQuery()
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        if (
            await roleDbSet.CountAsync(x => rolesToProcess.Contains(x.Id))
            != rolesToProcess.Count
        )
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        ICollection<UserRole> currentUserRoles = currentUser.UserRoles!;
        if (rolesToProcess.Any(x => currentUserRoles.All(p => p.RoleId != x)))
        {
            throw new ArgumentException(
                $"{nameof(roleIds)} is not existed in user {nameof(user.Id)}"
            );
        }

        IList<UserRole> userRoles =
        [
            .. currentUserRoles.Where(x => rolesToProcess.Contains(x.RoleId)),
        ];

        List<UserClaim> userClaims = userRoles
            .Select(x => x.Role)
            .SelectMany(x => x!.RoleClaims!)
            .SelectMany(x => x.UserClaims!).ToList();

        userRoleDbSet.RemoveRange(userRoles);
        await context.SaveChangesAsync();
        if (userClaims.Count > 0)
        {
            userClaimsDbSet.RemoveRange(userClaims);
            await context.SaveChangesAsync();
        }
    }

    public async Task AssignClaimsToUserAsync(User user, IEnumerable<UserClaim> claims)
    {
        List<UserClaim> userClaimsToInsert = claims.ToList();
        if (userClaimsToInsert.Count == 0)
        {
            return;
        }

        _ = await GetUserAsync(user.Id);
        await userClaimsDbSet.AddRangeAsync(userClaimsToInsert);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDefaultUserClaimsAsync(IEnumerable<UserClaim> defaultUserClaims)
    {
        userClaimsDbSet.UpdateRange(defaultUserClaims);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserClaimsAsync(User user, IEnumerable<UserClaim>? claims)
    {
        List<UserClaim>? claimsToProcess = claims?.ToList();
        if (claimsToProcess?.Count <= 0)
        {
            return;
        }

        User? currentUser = Guard.Against.NotFound(
            $"{user}",
            await userDbSet
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        List<UserClaim> customClaims =
        [
            ..currentUser.UserClaims!.Where(x =>
                x.Type == UserClaimType.Custom
            )
        ];

        var toInsert = claimsToProcess!
            .ExceptBy(customClaims.Select(cc => cc.Id), c => c.Id)
            .ToList();

        var toUpdate = customClaims
            .IntersectBy(claimsToProcess!.Select(c => c.Id), cc => cc.Id)
            .ToList();

        var toRemove = customClaims
            .Where(cc => cc.RoleClaimId == null)
            .ExceptBy(claimsToProcess!.Select(c => c.Id), cc => cc.Id)
            .ToList();

        ProcessUserClaimUpdate(toUpdate, claimsToProcess!);

        if (toRemove.Count > 0)
        {
            userClaimsDbSet.RemoveRange(toRemove);
        }

        if (toUpdate.Count > 0)
        {
            userClaimsDbSet.UpdateRange(toUpdate);
        }

        if (toInsert.Count > 0)
        {
            await userClaimsDbSet.AddRangeAsync(toInsert);
        }

        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromUserAsync(User user, IEnumerable<Ulid> claimIds)
    {
        List<Ulid> ids = claimIds.ToList();
        if (ids.Count <= 0)
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userDbSet
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> currentUserClaims = currentUser.UserClaims!.Where(p =>
            p.Type == UserClaimType.Custom
        );
        if (ids.Exists(x => currentUserClaims.All(p => p.Id != x)))
        {
            throw new ArgumentException(
                $"{nameof(claimIds)} is not existed in user {nameof(user.Id)}."
            );
        }

        IEnumerable<UserClaim> claimsToRemove = currentUserClaims.Where(x =>
            ids.Contains(x.Id)
        );

        userClaimsDbSet.RemoveRange(claimsToRemove);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsFromUserAsync(User user, IEnumerable<UserClaim> claims)
    {
        IEnumerable<Ulid> userClaims = claims.Select(x => x.Id);
        await RemoveClaimsFromUserAsync(user, userClaims);
    }

    public async Task<List<Role>> GetUserRoles(Ulid userId) =>
        await userRoleDbSet.Where(x => x.UserId == userId).Select(x => x.Role!).ToListAsync();

    public async Task<List<UserClaim>> GetUserClaims(Ulid userId) =>
        await userClaimsDbSet.Where(x => x.UserId == userId).ToListAsync();

    public async Task<bool> HasUserRolesAsync(Ulid id, IEnumerable<string> roleNames) =>
        await userDbSet.AnyAsync(x =>
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

        return userClaims.Exists(x => claims.Any(p => p.Key == x.ClaimType && p.Value == x.ClaimValue));
    }

    public async Task<bool> HasUserPermissionAsync(Ulid id, IEnumerable<string> claims)
    {
        List<string> permissions = await UserClaims
            .Where(x => x.UserId == id && x.ClaimType == ClaimTypes.Permission)
            .Select(x => x.ClaimValue)
            .ToListAsync();

        var claimsSet = new HashSet<string>(claims);
        if (permissions.Exists(claimsSet.Contains))
        {
            return true;
        }

        var permissionsSet = new HashSet<string>(permissions);
        return Credential
            .permissions.SelectMany(group => group)
            .Any(permissions =>
                permissionsSet.Contains(permissions.Key)
                && permissions.Value.Distinct().Any(p => claimsSet.Contains(p))
            );
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
            await userDbSet.Where(x => x.Id == id).FirstOrDefaultAsync(),
            nameof(id)
        );

    private static void ProcessUserClaimUpdate(
        IEnumerable<UserClaim> claimsToUpdate,
        List<UserClaim> claimsToProcess
    )
    {
        foreach (UserClaim claim in claimsToUpdate)
        {
            var correspondenceClaim = claimsToProcess.Find(x => x.Id == claim.Id);

            if (correspondenceClaim == null)
            {
                continue;
            }

            claim.ClaimValue = correspondenceClaim.ClaimValue!;
        }
    }
}