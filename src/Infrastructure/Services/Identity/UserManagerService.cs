using System.Data;
using System.Data.Common;
using Application.Common.Interfaces.Services.Identity;
using Ardalis.GuardClauses;
using Contracts.Dtos.Models;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Services.Identity;

public class UserManagerService(
    IRoleManagerService roleManagerService,
    IDbContext context,
    ILogger logger
) : IUserManagerService
{
    private readonly DbSet<UserRole> userRoleContext = context.Set<UserRole>();
    public DbSet<UserRole> UserRoles => userRoleContext;

    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<UserClaim> userClaimsContext = context.Set<UserClaim>();
    public DbSet<UserClaim> UserClaims => userClaimsContext;

    private readonly DbSet<User> userContext = context.Set<User>();

    public async Task CreateUserAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaimType> claims,
        DbTransaction? transaction = null
    )
    {
        try
        {
            if (transaction == null)
            {
                await context.DatabaseFacade.BeginTransactionAsync();
            }
            else
            {
                await context.UseTransactionAsync(transaction);
            }

            await AddRoleToUserAsync(user, [.. roleIds]);
            await AddClaimsToUserAsync(user, claims);

            if (transaction == null)
            {
                await context.DatabaseFacade.CommitTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Error in method {MethodName}. Exception Type: {ExceptionType}. Error Message: {ErrorMessage}. StackTrace: {StackTrace}",
                nameof(CreateUserAsync),
                ex.GetType().Name,
                ex.Message,
                ex.StackTrace
            );

            if (transaction == null)
            {
                await context.DatabaseFacade.RollbackTransactionAsync();
            }
            throw;
        }
    }

    public async Task UpdateUserAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaimType> claims,
        DbTransaction? transaction = null
    )
    {
        try
        {
            if (transaction == null)
            {
                await context.DatabaseFacade.BeginTransactionAsync();
            }
            else
            {
                await context.UseTransactionAsync(transaction);
            }

            // update role for user
            await UpdateRolesToUserAsync(user, roleIds);

            // update default user claim
            var defautClaims = userClaimsContext
                .Where(x => x.UserId == user.Id && x.Type == KindaUserClaimType.Default)
                .ToDictionary(x => x.ClaimType, x => x.ClaimValue);

            await ReplaceDefaultClaimsToUserAsync(user, defautClaims);

            // update custom user claim
            await UpdateClaimsToUserAsync(user, claims);

            if (transaction == null)
            {
                await context.DatabaseFacade.CommitTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Error in method {MethodName}. Exception Type: {ExceptionType}. Error Message: {ErrorMessage}. StackTrace: {StackTrace}",
                nameof(UpdateUserAsync),
                ex.GetType().Name,
                ex.Message,
                ex.StackTrace
            );

            if (transaction == null)
            {
                await context.DatabaseFacade.RollbackTransactionAsync();
            }
            throw;
        }
    }

    public async Task AddRoleToUserAsync(User user, List<Ulid> roleIds)
    {
        if (roleIds == null || roleIds.Count == 0)
        {
            return;
        }

        User? currentUser = await userContext
            .Where(x => x.Id == user.Id)
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync();

        Guard.Against.NotFound($"{user.Id}", currentUser, nameof(user));

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count)
        {
            throw new ArgumentException($"{nameof(roleIds)} is not found", nameof(roleIds));
        }

        if (await userRoleContext.AnyAsync(x => x.UserId == user.Id && roleIds.Contains(x.RoleId)))
        {
            throw new ArgumentException($"{nameof(roleIds)} is existence in user", nameof(roleIds));
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];
        List<Ulid> newRoleIds = roleIds.FindAll(x => !currentUserRoles.Any(p => p.RoleId == x));

        await userRoleContext.AddRangeAsync(
            newRoleIds.Select(x => new UserRole { RoleId = x, UserId = currentUser.Id })
        );
        await context.SaveChangesAsync();

        //derive all role claims for users if user is assigned specific role.
        IEnumerable<RoleClaim> roleClaims = await roleManagerService.GetClaimsByRolesAsync(
            newRoleIds
        );
        IEnumerable<UserClaim> userClaims = roleClaims.Select(x => new UserClaim
        {
            UserId = user.Id,
            ClaimType = x.ClaimType,
            ClaimValue = x.ClaimValue,
            RoleClaimId = x.Id,
            Type = KindaUserClaimType.Custom,
        });

        await userClaimsContext.AddRangeAsync(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRolesToUserAsync(User user, IEnumerable<Ulid>? roleIds)
    {
        roleIds ??= [];
        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count())
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];

        IEnumerable<Ulid> shouldRemoving = currentUserRoles
            .Where(x => !roleIds.Contains(x.RoleId))
            .Select(x => x.RoleId);
        IEnumerable<Ulid> shouldInserting = roleIds.Where(x =>
            !currentUserRoles.Any(p => p.RoleId == x)
        );

        await RemoveRoleFromUserAsync(currentUser, shouldRemoving);
        await AddRoleToUserAsync(currentUser, shouldInserting.ToList());
    }

    public async Task RemoveRoleFromUserAsync(User user, IEnumerable<Ulid> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserRoles)!
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x!.RoleClaims)
                .ThenInclude(x => x.UserClaims)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count())
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];
        if (roleIds.All(x => new HashSet<Ulid>(currentUserRoles.Select(p => p.RoleId)).Contains(x)))
        {
            throw new ArgumentException(
                $"{nameof(roleIds)} is not existed in user {nameof(user.Id)}"
            );
        }

        IEnumerable<UserRole> userRoles = currentUserRoles.Where(x => roleIds.Contains(x.RoleId));

        IEnumerable<UserClaim> userClaims = userRoles
            .Select(x => x.Role)
            .SelectMany(x => x!.RoleClaims)
            .SelectMany(x => x.UserClaims!);

        userRoleContext.RemoveRange(userRoles);
        userClaimsContext.RemoveRange(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task AddClaimsToUserAsync(User user, IEnumerable<UserClaimType> claims)
    {
        User currentUser = await GetUserAsync(user.Id);

        IEnumerable<UserClaim> userClaims = claims.Select(x => new UserClaim
        {
            ClaimType = x.ClaimType!,
            ClaimValue = x.ClaimValue!,
            UserId = currentUser.Id,
            Type = x.Type,
        });

        await userClaimsContext.AddRangeAsync(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task ReplaceDefaultClaimsToUserAsync(User user, Dictionary<string, string> claims)
    {
        IEnumerable<UserClaim> userClaims = await userClaimsContext
            .Where(x => x.UserId == user.Id && x.Type == KindaUserClaimType.Default)
            .ToListAsync();

        foreach (UserClaim claim in userClaims)
        {
            string? correctspondingClaim = claims.GetValueOrDefault(claim.ClaimType);
            if (string.IsNullOrWhiteSpace(correctspondingClaim))
            {
                continue;
            }

            claim.ClaimValue = correctspondingClaim;
        }

        userClaimsContext.UpdateRange(userClaims);
        await context.SaveChangesAsync();
    }

    public async Task UpdateClaimsToUserAsync(User user, IEnumerable<UserClaimType> claims)
    {
        if (!claims.Any())
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

        IEnumerable<UserClaim> customClaims = currentUser
            .UserClaims!.Where(x => x.Type == KindaUserClaimType.Custom)
            .ToList();
        IEnumerable<UserClaimType> updatingClaims = claims.Where(x => x.Id != null);

        IEnumerable<UserClaimType> shouldInserting = claims.Where(x =>
            !customClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<UserClaimType> shouldUpdating = updatingClaims.Where(x =>
            customClaims.Any(p => p.Id == x.Id)
        );

        IEnumerable<Ulid> shouldRemoving = customClaims
            .Where(x => !updatingClaims.Select(x => x.Id).Contains(x.Id))
            .Select(x => x.Id);

        foreach (UserClaim claim in customClaims)
        {
            var correspondenceClaim = shouldUpdating.FirstOrDefault(x => x.Id == claim.Id);

            if (correspondenceClaim == null)
            {
                continue;
            }
            claim.ClaimValue = correspondenceClaim.ClaimValue!;
        }

        await RemoveClaimsToUserAsync(currentUser, shouldRemoving);
        userClaimsContext.UpdateRange(currentUser.UserClaims!);
        await context.SaveChangesAsync();
        await AddClaimsToUserAsync(currentUser, shouldInserting);
    }

    public async Task RemoveClaimsToUserAsync(User user, IEnumerable<Ulid> claimIds)
    {
        if (!claimIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext
                .Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims!.Where(p => p.Type == KindaUserClaimType.Custom))
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> currentUserClaims = currentUser.UserClaims ?? [];

        if (!claimIds.All(x => new HashSet<Ulid>(currentUserClaims.Select(p => p.Id)).Contains(x)))
        {
            throw new ArgumentException(
                $"{nameof(claimIds)} is not existed in user {nameof(user.Id)}."
            );
        }

        IEnumerable<UserClaim> claims = currentUserClaims.Where(x => claimIds.Contains(x.Id));

        userClaimsContext.RemoveRange(claims);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Role>> GetRolesInUser(Ulid userId) =>
        await userRoleContext.Where(x => x.UserId == userId).Select(x => x.Role!).ToListAsync();

    public async Task<IEnumerable<UserClaim>> GetClaimsInUser(Ulid userId) =>
        await userClaimsContext.Where(x => x.UserId == userId).ToListAsync();

    public async Task<bool> HasRolesInUserAsync(Ulid id, IEnumerable<string> roleNames) =>
        await userContext.AnyAsync(x =>
            x.Id == id && x.UserRoles!.Any(p => roleNames.Contains(p.Role!.Name))
        );

    public async Task<bool> HasClaimsInUserAsync(
        Ulid id,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        var userClaims = await userContext
            .Where(x => x.Id == id)
            .SelectMany(x => x.UserClaims!)
            .ToListAsync();

        return userClaims.Any(x => claims.Contains(new(x.ClaimType, x.ClaimValue)));
    }

    public async Task<bool> HasClaimsAndRoleInUserAsync(
        Ulid id,
        IEnumerable<string> roles,
        IEnumerable<KeyValuePair<string, string>> claims
    )
    {
        bool isHaRole = await userContext.AnyAsync(x =>
            x.Id == id && x.UserRoles!.Any(p => roles.Contains(p.Role!.Name))
        );

        bool isHasClaim = (await userClaimsContext.Where(x => x.UserId == id).ToListAsync()).Any(
            x => claims.Contains(new(x.ClaimType, x.ClaimValue))
        );

        return isHaRole && isHasClaim;
    }

    private async Task<User> GetUserAsync(Ulid id) =>
        Guard.Against.NotFound(
            $"{id}",
            await userContext.Where(x => x.Id == id).FirstOrDefaultAsync(),
            nameof(id)
        );
}
