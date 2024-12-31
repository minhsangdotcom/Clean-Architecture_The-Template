using Application.Features.Common.Projections.Roles;
using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleMapping : Profile
{
    public UpdateRoleMapping()
    {
        CreateMap<UpdateRole, Role>()
            .ForMember(dest => dest.RoleClaims, opt => opt.Ignore())
            .IncludeBase<RoleModel, Role>();

        CreateMap<Role, UpdateRoleResponse>();
    }
}
