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

    Task CreateAsync(User user, IEnumerable<Ulid> roleIds, IEnumerable<UserClaim>? claims = null);

    Task UpdateAsync(User user, IEnumerable<Ulid>? roleIds, IEnumerable<UserClaim>? claims = null);

    Task AssignRolesToUserAsync(User user, IEnumerable<Ulid> roleIds);

    Task UpdateUserRolesAsync(User user, IEnumerable<Ulid>? roleIds);

    Task RemoveRolesFromUserAsync(User user, IEnumerable<Ulid> roleIds);

    Task AssignClaimsToUserAsync(User user, IEnumerable<UserClaim> claims);

    Task UpdateUserClaimsAsync(User user, IEnumerable<UserClaim> claims);

    Task UpdateDefaultUserClaimsAsync(IEnumerable<UserClaim> defaultUserClaims);

    Task RemoveClaimsFromUserAsync(User user, IEnumerable<Ulid> claimIds);

    Task RemoveClaimsFromUserAsync(User user, IEnumerable<UserClaim> claims);

    Task<List<Role>> GetUserRoles(Ulid userId);

    Task<List<UserClaim>> GetUserClaims(Ulid userId);

    Task<bool> HasUserRolesAsync(Ulid id, IEnumerable<string> roleNames);

    Task<bool> HasUserClaimsAsync(Ulid id, IEnumerable<KeyValuePair<string, string>> claims);

    Task<bool> HasUserClaimsAndRolesAsync(
        Ulid id,
        IEnumerable<string> roles,
        IEnumerable<KeyValuePair<string, string>> claims
    );
}
