using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services;

public interface IUserManagerService : IScope
{
    public DbSet<UserRole> UserRoles { get; }

    public DbSet<Role> Roles { get; }

    public DbSet<UserClaim> UserClaims { get; }

    /// <summary>
    /// Add existence roles to specific user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    Task AddRoleToUserAsync(User user, List<Ulid> roleIds);

    Task UpdateRolesToUserAsync(User user, IEnumerable<Ulid>? roleIds);

    Task RemoveRoleFromUserAsync(User user, IEnumerable<Ulid> roleIds);

    Task AddClaimsToUserAsync(User user, IEnumerable<UserClaimType> claims);

    Task UpdateClaimsToUserAsync(User user, IEnumerable<UserClaimType> claims);

    Task ReplaceDefaultClaimsToUserAsync(User user, Dictionary<string, string> claims);

    Task RemoveClaimsToUserAsync(User user, IEnumerable<Ulid> claimIds);

    Task<IEnumerable<Role>> GetRolesInUser(Ulid userId);

    Task<IEnumerable<UserClaim>> GetClaimsInUser(Ulid userId);

    Task<bool> HasRolesInUserAsync(Ulid id, IEnumerable<string> roleNames);

    Task<bool> HasClaimsInUserAsync(Ulid id, Dictionary<string, string> claimNames);

    Task<bool> HasClaimsAndRoleInUserAsync(Ulid id, IEnumerable<string> roles, Dictionary<string, string> claims);
}