using System.Data.Common;
using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services;

public interface IUserManagerService : IScope
{
    public DbSet<UserRole> UserRoles { get; }

    public DbSet<Role> Roles { get; }

    public DbSet<UserClaim> UserClaims { get; }

    Task CreateUserAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaimType> claims,
        DbTransaction? transaction = null
    );

    Task UpdateUserAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaimType> claims,
        DbTransaction? transaction = null
    );

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

    Task<bool> HasClaimsInUserAsync(Ulid id, IEnumerable<KeyValuePair<string, string>> claims);

    Task<bool> HasClaimsAndRoleInUserAsync(
        Ulid id,
        IEnumerable<string> roles,
        IEnumerable<KeyValuePair<string, string>> claims
    );
}
