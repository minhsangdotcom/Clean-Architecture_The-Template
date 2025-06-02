using Domain.Aggregates.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services.Identity;

public interface IRoleManagerService
{
    public DbSet<Role> Roles { get; }

    public DbSet<RoleClaim> RoleClaims { get; }

    // get role only
    Task<Role?> GetByIdAsync(Ulid id);

    // ger role with claims
    Task<Role?> FindByIdAsync(Ulid id);

    Task<Role?> FindByNameAsync(string name);

    Task<List<Role>> ListAsync();

    Task<Role> CreateAsync(Role role);

    Task<IList<Role>> CreateRangeAsync(IEnumerable<Role> roles);

    Task<Role> UpdateAsync(Role role, IEnumerable<RoleClaim>? roleClaims);

    Task DeleteAsync(Role role);

    Task<IList<KeyValuePair<string, string>>> GetRolePermissionClaimsAsync();

    Task UpdateRoleClaimsAsync(IEnumerable<RoleClaim> roleClaims, Role role);

    Task AssignClaimsToRoleAsync(Role role, IEnumerable<RoleClaim> claims);

    Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<RoleClaim> roleClaims);

    Task<List<RoleClaim>> GetRoleClaimsAsync(Ulid roleId);

    Task<List<RoleClaim>> GetRoleClaimsAsync(IEnumerable<Ulid> roleIds);

    Task<bool> HasRoleClaimAsync(Ulid roleId, Ulid claimId);

    Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName);

    Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName, string claimValue);

    Task<bool> HasRoleClaimAsync(Ulid roleId, IEnumerable<KeyValuePair<string, string>> claims);
}
