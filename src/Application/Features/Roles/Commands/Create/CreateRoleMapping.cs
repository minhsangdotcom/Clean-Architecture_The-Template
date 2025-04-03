using Application.Features.Common.Mapping.Roles;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Commands.Create;

public static class CreateRoleMapping
{
    public static Role ToRole(this CreateRoleCommand roleCommand) =>
        new()
        {
            Name = roleCommand.Name.ToSnakeCase().ToUpper(),
            Description = roleCommand.Description,
            RoleClaims = roleCommand.RoleClaims?.ToListRoleClaim(),
        };

    public static CreateRoleResponse ToCreateRoleResponse(this Role role) =>
        new()
        {
            Id = role.Id,
            CreatedAt = role.CreatedAt,
            Name = role.Name,
            Description = role.Description,
            Guard = role.Guard,
            RoleClaims = role.RoleClaims?.ToListRoleClaimDetailProjection(),
        };
}
