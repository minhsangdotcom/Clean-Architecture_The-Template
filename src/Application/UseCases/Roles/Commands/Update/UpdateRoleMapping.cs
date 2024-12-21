using AutoMapper;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleMapping : Profile
{
    public UpdateRoleMapping()
    {
        CreateMap<UpdateRole, Role>()
            .ForMember(dest => dest.RoleClaims, opt => opt.Ignore())
            .AfterMap(
                (src, dest) =>
                {
                    dest.Name = src.Name.ToSnakeCase().ToUpper();
                }
            );

        CreateMap<Role, UpdateRoleResponse>();
    }
}
