using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleMapping : Profile
{
    public UpdateRoleMapping()
    {
        CreateMap<UpdateRole, Role>()
            .ForMember(dest => dest.RoleClaims, opt => opt.Ignore());

        CreateMap<Role, UpdateRoleResponse>();
    }
}