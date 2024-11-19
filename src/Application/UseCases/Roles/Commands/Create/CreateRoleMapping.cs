using Application.UseCases.Projections.Roles;
using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleMapping : Profile
{
    public CreateRoleMapping()
    {
        CreateMap<CreateRoleCommand, Role>()
        .ForMember(dest => dest.RoleClaims, opt => opt.MapFrom(src => src.Claims))
        .AfterMap((src,dest) =>
        {
            dest.Name = src.Name!.ToUpper();
        });

        CreateMap<RoleClaimModel, RoleClaim>();
            // .ForMember(dest => dest.Id, opt =>
            // {
            //     opt.PreCondition(x => x.Id == null);
            //     opt.Ignore();
            // });

        CreateMap<RoleClaim, RoleClaimDetailProjection>();
        CreateMap<Role, CreateRoleResponse>();
    }
}