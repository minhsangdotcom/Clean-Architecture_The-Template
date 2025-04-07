using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services.Identity;

public interface IRoleManagerService : IScope
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

    Task<IList<RoleClaim>> GetRolePermissionClaimsAsync();

    Task UpdateRoleClaimsAsync(IEnumerable<RoleClaim> roleClaims, Role role);

    Task AssignClaimsToRoleAsync(Role role, IEnumerable<KeyValuePair<string, string>> claims);

    Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<KeyValuePair<string, string>> roleClaims);

    Task<List<RoleClaim>> GetRoleClaimsAsync(Ulid roleId);

    Task<List<RoleClaim>> GetRoleClaimsAsync(IEnumerable<Ulid> roleIds);

    Task<bool> HasRoleClaimAsync(Ulid roleId, Ulid claimId);

    Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName);

    Task<bool> HasRoleClaimAsync(Ulid roleId, string claimName, string ClaimValue);

    Task<bool> HasRoleClaimAsync(Ulid roleId, IEnumerable<KeyValuePair<string, string>> claims);
}
