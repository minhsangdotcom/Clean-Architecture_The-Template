using Amazon.Runtime.Internal.Util;
using Application.Common.Interfaces.Services;
using Ardalis.GuardClauses;
using AutoMapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserManagerService(
    IRoleManagerService roleManagerService,
    TheDbContext context
) : IUserManagerService
{
    private readonly DbSet<UserRole> userRoleContext = context.Set<UserRole>();
    public DbSet<UserRole> UserRoles => userRoleContext;

    private readonly DbSet<Role> roleContext = context.Set<Role>();
    public DbSet<Role> Roles => roleContext;

    private readonly DbSet<UserClaim> userClaimsContext = context.Set<UserClaim>();
    public DbSet<UserClaim> UserClaims => userClaimsContext;

    private readonly DbSet<User> userContext = context.Set<User>();

    public async Task AddRoleToUserAsync(User user, List<Ulid> roleIds)
    {
        if (roleIds == null || roleIds.Count == 0)
        {
            return;
        }

        User? currentUser = await userContext.Where(x => x.Id == user.Id)
            .Include(x => x.UserRoles).FirstOrDefaultAsync();

        Guard.Against.NotFound($"{user.Id}", currentUser, nameof(user));

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count)
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid", nameof(roleIds));
        }

        if (await userRoleContext.AnyAsync(
                x =>
                    x.UserId == user.Id
                    && roleIds.Contains(x.RoleId)
        ))
        {
            throw new ArgumentException($"{nameof(roleIds)} is existence in user", nameof(roleIds));
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];
        List<Ulid> newRoleIds = roleIds.FindAll(x => !currentUserRoles.Any(p => p.RoleId == x));

        try
        {
            await context.Database.BeginTransactionAsync();
            await userRoleContext.AddRangeAsync(newRoleIds.Select(x => new UserRole { RoleId = x, UserId = currentUser.Id }));
            await context.SaveChangesAsync();

            //derive all role claims for users if user is assigned specific role.
            IEnumerable<RoleClaim> roleClaims = await roleManagerService.GetClaimsByRolesAsync(newRoleIds);
            IEnumerable<UserClaim> userClaims = roleClaims.Select(x => new UserClaim
            {
                UserId = user.Id,
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue
            });

            await userClaimsContext.AddRangeAsync(userClaims);
            await context.SaveChangesAsync();
            await context.Database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateRolesToUserAsync(User user, IEnumerable<Ulid>? roleIds)
    {
        roleIds ??= [];
        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext.Where(x => x.Id == user.Id)
            .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count())
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];

        IEnumerable<Ulid> shouldRemoving = currentUserRoles.Where(x => !roleIds.Contains(x.RoleId)).Select(x => x.RoleId);
        IEnumerable<Ulid> shouldInserting = roleIds.Where(x => !currentUserRoles.Any(p => p.RoleId == x));

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
            await userContext.Where(x => x.Id == user.Id)
            .Include(x => x.UserRoles).FirstOrDefaultAsync(),
            nameof(user)
        );

        if (await roleContext.CountAsync(x => roleIds.Contains(x.Id)) != roleIds.Count())
        {
            throw new ArgumentException($"{nameof(roleIds)} is invalid");
        }

        IEnumerable<UserRole> currentUserRoles = currentUser.UserRoles ?? [];
        if (roleIds.All(x => new HashSet<Ulid>(currentUserRoles.Select(p => p.RoleId)).Contains(x)))
        {
            throw new ArgumentException($"{nameof(roleIds)} is not existed in user {nameof(user.Id)}");
        }

        userRoleContext.RemoveRange(currentUserRoles.Where(x => roleIds.Contains(x.RoleId)));
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
        IEnumerable<UserClaim> userClaims = await userClaimsContext.Where(
            x =>
                x.UserId == user.Id
                    && x.Type == KindaUserClaimType.Default
        ).ToListAsync();

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
        User? currentUser = Guard.Against.NotFound(
            $"{user.Id}",
           await userContext.Where(x => x.Id == user.Id)
                .Include(x => x.UserClaims!.Where(p => p.Type == KindaUserClaimType.Custom))
                .AsNoTracking()
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> currentUserClaims = currentUser.UserClaims ?? [];
        IEnumerable<UserClaimType> updatingClaims = claims.Where(x => x.Id != null);

        IEnumerable<UserClaimType> shouldInserting = claims.Where(x => !currentUserClaims.Any(p => p.Id == x.Id));
        IEnumerable<UserClaimType> shouldUpdating = updatingClaims.Where(
            x => currentUserClaims.Any(p => p.Id == x.Id)
        );
        IEnumerable<Ulid> shouldRemoving = currentUserClaims.Where(
            x => !updatingClaims.Select(x => x.Id).Contains(x.Id)
        ).Select(x => x.Id);

        await RemoveClaimsToUserAsync(user, shouldRemoving);
        if (shouldUpdating.Any())
        {
            foreach (UserClaim claim in currentUserClaims)
            {
                var c = shouldUpdating.FirstOrDefault(x => x.Id == claim.Id);

                if (c == null)
                {
                    continue;
                }
                userClaimsContext.Entry(claim).State = EntityState.Modified;
                claim.ClaimValue = c.ClaimValue!;
            }
            userClaimsContext.UpdateRange(currentUserClaims);
        }
        await AddClaimsToUserAsync(currentUser, shouldInserting);
        await context.SaveChangesAsync();
    }

    public async Task RemoveClaimsToUserAsync(User user, IEnumerable<Ulid> claimIds)
    {
        if (!claimIds.Any())
        {
            return;
        }

        User currentUser = Guard.Against.NotFound(
            $"{user.Id}",
            await userContext.Where(x => x.Id == user.Id)
            .Include(x => x.UserClaims!.Where(p => p.Type == KindaUserClaimType.Custom))
                .FirstOrDefaultAsync(),
            nameof(user)
        );

        IEnumerable<UserClaim> currentUserClaims = currentUser.UserClaims ?? [];

        if (!claimIds.All(x => new HashSet<Ulid>(currentUserClaims.Select(p => p.Id)).Contains(x)))
        {
            throw new ArgumentException($"{nameof(claimIds)} is not existed in user {nameof(user.Id)}.");
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
        await userContext.AnyAsync(
                x =>
                    x.Id == id
                    && x.UserRoles!.Any(p => roleNames.Contains(p.Role!.Name)
            )
        );

    public async Task<bool> HasClaimsInUserAsync(Ulid id, IEnumerable<string> claimNames) =>
        await userContext.AnyAsync(
                x =>
                    x.Id == id
                    && x.UserClaims!.Any(p => claimNames.Contains(p.ClaimValue)
        )
    );

    public async Task<bool> HasClaimsAndRoleInUserAsync(Ulid id, IEnumerable<string> roles, IEnumerable<string> claims) =>
        await userContext.AnyAsync(
            x =>
                x.Id == id
                && (
                    x.UserRoles!.Any(p => roles.Contains(p.Role!.Name))
                    || x.UserClaims!.Any(p => claims.Contains(p.ClaimValue))
                )
        );

    private async Task<User> GetUserAsync(Ulid id) =>
         Guard.Against.NotFound(
            $"{id}",
            await userContext.Where(x => x.Id == id).FirstOrDefaultAsync(),
            nameof(id)
        );
}