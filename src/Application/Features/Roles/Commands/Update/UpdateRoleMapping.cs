using Application.Features.Common.Mapping.Roles;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Commands.Update;

// public class UpdateRoleMapping : Profile
// {
//     public UpdateRoleMapping()
//     {
//         CreateMap<RoleUpdateRequest, Role>()
//             .ForMember(dest => dest.RoleClaims, opt => opt.Ignore())
//             .IncludeBase<RoleModel, Role>();

//         CreateMap<Role, UpdateRoleResponse>();
//     }
// }

public static class UpdateRoleMapping
{
    public static Role FromUpdateRole(this Role role, RoleUpdateRequest RoleUpdateRequest)
    {
        role.Name = RoleUpdateRequest.Name!;
        role.Description = RoleUpdateRequest.Description;
        return role;
    }

    public static UpdateRoleResponse ToUpdateRoleResponse(this Role role)
    {
        UpdateRoleResponse roleResponse = new();
        roleResponse.MappingFrom(role);
        return roleResponse;
    }
}
