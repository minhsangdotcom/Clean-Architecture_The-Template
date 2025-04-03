using Application.Features.Common.Mapping.Roles;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Commands.Update;

// public class UpdateRoleMapping : Profile
// {
//     public UpdateRoleMapping()
//     {
//         CreateMap<UpdateRole, Role>()
//             .ForMember(dest => dest.RoleClaims, opt => opt.Ignore())
//             .IncludeBase<RoleModel, Role>();

//         CreateMap<Role, UpdateRoleResponse>();
//     }
// }

public static class UpdateRoleMapping
{
    public static Role ToRole(this UpdateRole updateRole) =>
        new()
        {
            Name = updateRole.Name.ToSnakeCase().ToUpper(),
            Description = updateRole.Description,
        };

    public static UpdateRoleResponse ToUpdateRoleResponse(this Role role) =>
        new()
        {
            Name = role.Name.ToSnakeCase().ToUpper(),
            Description = role.Description,
            RoleClaims = role.RoleClaims?.ToListRoleClaimDetailProjection(),
        };
}
