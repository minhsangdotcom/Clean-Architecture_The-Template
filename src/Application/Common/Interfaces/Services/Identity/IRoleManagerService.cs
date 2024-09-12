using Application.Common.Interfaces.Registers;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Services.Identity;

public interface IRoleManagerService : IScope
{
    public DbSet<Role> Roles { get; }

    public DbSet<RoleClaim> RoleClaims { get; }

    Task<Role> CreateRoleAsync(Role role);

    Task<Role> UpdateRoleAsync(Role role, IEnumerable<RoleClaim>? roleClaims);

    Task DeleteRoleAsync(Role role);

    Task<IEnumerable<Role>> ListAsync();

    Task<Role?> FindByIdAsync(Ulid id);

    Task<Role?> FindByNameAsync(string name);

    Task UpdateRoleClaimAsync(Role role, IEnumerable<RoleClaim> roleClaims);

    Task AddClaimsToRoleAsync(Role role, IEnumerable<KeyValuePair<string, string>> claims);

    Task RemoveClaimsFromRoleAsync(Role role, IEnumerable<Ulid> claimIds);

    Task<IEnumerable<RoleClaim>> GetClaimsByRoleAsync(Ulid roleId);

    Task<IEnumerable<RoleClaim>> GetClaimsByRolesAsync(IEnumerable<Ulid> roleIds);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, Ulid claimId);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, string claimName, string ClaimValue);

    Task<bool> HasClaimInRoleAsync(Ulid roleId, IEnumerable<KeyValuePair<string, string>> claims);
}