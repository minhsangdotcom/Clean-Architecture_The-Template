using System.Data.Common;
using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services.Identity;

public interface IUserManagerService : IScope
{
    public DbSet<UserRole> UserRoles { get; }

    public DbSet<Role> Roles { get; }

    public DbSet<UserClaim> UserClaims { get; }

    Task CreateUserAsync(
        User user,
        IEnumerable<Ulid> roleIds,
        IEnumerable<UserClaim>? claims = null,
        DbTransaction? transaction = null
    );

    Task UpdateUserAsync(
        User user,
        IEnumerable<Ulid>? roleIds,
        IEnumerable<UserClaim>? claims = null,
        DbTransaction? transaction = null
    );

    Task AddRoleToUserAsync(User user, IEnumerable<Ulid> roleIds);

    Task UpdateRolesToUserAsync(User user, IEnumerable<Ulid>? roleIds);

    Task RemoveRoleFromUserAsync(User user, IEnumerable<Ulid> roleIds);

    Task AddClaimsToUserAsync(User user, IEnumerable<UserClaim> claims);

    Task UpdateClaimsToUserAsync(User user, IEnumerable<UserClaim> claims);

    Task ReplaceDefaultClaimsToUserAsync(IEnumerable<UserClaim> defaultUserClaims);

    Task RemoveClaimsToUserAsync(User user, IEnumerable<Ulid> claimIds);

    Task RemoveClaimsToUserAsync(User user, IEnumerable<UserClaim> claims);

    Task<List<Role>> GetRolesInUser(Ulid userId);

    Task<List<UserClaim>> GetClaimsInUser(Ulid userId);

    Task<bool> HasRolesInUserAsync(Ulid id, IEnumerable<string> roleNames);

    Task<bool> HasClaimsInUserAsync(Ulid id, IEnumerable<KeyValuePair<string, string>> claims);

    Task<bool> HasClaimsAndRoleInUserAsync(
        Ulid id,
        IEnumerable<string> roles,
        IEnumerable<KeyValuePair<string, string>> claims
    );
}
