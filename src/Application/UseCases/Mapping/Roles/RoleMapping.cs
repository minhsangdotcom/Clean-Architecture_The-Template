using Application.UseCases.Projections.Roles;
using AutoMapper;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.UseCases.Mapping.Roles;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<RoleModel, Role>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name.ToSnakeCase().ToUpper())
            );

        CreateMap<RoleClaimModel, RoleClaim>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .AfterMap(
                (src, dest) =>
                {
                    if (src.Id != null)
                    {
                        dest.Id = src.Id.Value;
                    }
                }
            );
        CreateMap<RoleClaim, RoleClaimDetailProjection>();
    }
}
