using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Create;
using Application.Features.Roles.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using Contracts.Constants;
using Domain.Aggregates.Roles;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Constants;

namespace Application.SubcutaneousTests;

public partial class TestingFixture
{
    public async Task<Role?> FindRoleByIdAsync(Ulid id)
    {
        factory.ThrowIfNull();
        using var scope = factory!.Services.CreateScope();
        var roleManagerService = scope.ServiceProvider.GetRequiredService<IRoleManagerService>();
        return await roleManagerService.GetByIdAsync(id);
    }

    public async Task<Role?> FindRoleByIdIncludeRoleClaimsAsync(Ulid id)
    {
        factory.ThrowIfNull();
        using var scope = factory!.Services.CreateScope();
        var roleManagerService = scope.ServiceProvider.GetRequiredService<IRoleManagerService>();
        return await roleManagerService.FindByIdAsync(id);
    }

    public async Task<Role> CreateRoleAsync(string roleName)
    {
        CreateRoleCommand role =
            new()
            {
                Name = roleName,
                RoleClaims =
                [
                    new RoleClaimModel()
                    {
                        ClaimType = ClaimTypes.Permission,
                        ClaimValue = "create:user",
                    },
                    new RoleClaimModel()
                    {
                        ClaimType = ClaimTypes.Permission,
                        ClaimValue = "detail:user",
                    },
                ],
            };
        factory.ThrowIfNull();
        var response = await SendAsync(role);
        using var scope = factory!.Services.CreateScope();
        var roleManagerService = scope.ServiceProvider.GetRequiredService<IRoleManagerService>();
        return (await roleManagerService.FindByIdAsync(response.Id))!;
    }

    public UpdateRoleCommand ToUpdateRoleCommand(Role role)
    {
        return new()
        {
            RoleId = role.Id.ToString(),
            Role = new UpdateRole()
            {
                Name = role.Name,
                Description = role.Description,
                RoleClaims = role.RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        ClaimType = x.ClaimType,
                        ClaimValue = x.ClaimValue,
                        Id = x.Id,
                    })
                    .ToList(),
            },
        };
    }
}
