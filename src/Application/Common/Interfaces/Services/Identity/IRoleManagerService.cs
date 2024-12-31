using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Roles;
using Microsoft.EntityFrameworkCore;
using NRedisStack.Json.DataTypes;

namespace Application.Common.Interfaces.Services.Identity;

public interface IRoleManagerService : IScope
{
    public DbSet<Role> Roles { get; }

    public DbSet<RoleClaim> RoleClaims { get; }

    Task<Role> CreateRoleAsync(Role role);

    Task<IList<Role>> CreateRangeRoleAsync(IEnumerable<Role> roles);

    Task<Role> UpdateRoleAsync(Role role, IEnumerable<RoleClaim>? roleClaims);

    Task DeleteRoleAsync(Role role);

    Task<List<Role>> ListAsync();

    // get role only
    Task<Role?> GetByIdAsync(Ulid id);

    // ger role with claims
    Task<Role?> FindByIdAsync(Ulid id);

    Task<Role?> FindByNameAsync(string name);

    Task UpdateRoleClaimAsync(IEnumerable<RoleClaim> roleClaims, Role role);

    Task AddClaimsToRoleAsync(Role role, IEnumerable<KeyValuePair<string, string>> claims);

    Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<KeyValuePair<string, string>> roleClaims);

    Task<List<RoleClaim>> GetClaimsByRoleAsync(Ulid roleId);

    Task<List<RoleClaim>> GetClaimsByRolesAsync(IEnumerable<Ulid> roleIds);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, Ulid claimId);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName, string ClaimValue);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, IEnumerable<KeyValuePair<string, string>> claims);
}
