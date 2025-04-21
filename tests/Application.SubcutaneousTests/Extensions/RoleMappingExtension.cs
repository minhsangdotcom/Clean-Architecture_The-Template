using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Update;
using Domain.Aggregates.Roles;

namespace Application.SubcutaneousTests.Extensions;

public static class RoleMappingExtension
{
    public static UpdateRoleCommand ToUpdateRoleCommand(Role role)
    {
        return new()
        {
            RoleId = role.Id.ToString(),
            UpdateData = new RoleUpdateRequest()
            {
                Name = role.Name,
                Description = role.Description,
                RoleClaims =
                [
                    .. role.RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        ClaimType = x.ClaimType,
                        ClaimValue = x.ClaimValue,
                        Id = x.Id,
                    }),
                ],
            },
        };
    }
}
