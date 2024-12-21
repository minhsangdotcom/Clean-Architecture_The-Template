using Application.UseCases.Projections.Roles;
using AutoMapper;
using CaseConverter;
using Domain.Aggregates.Roles;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleMapping : Profile
{
    public CreateRoleMapping()
    {
        CreateMap<CreateRoleCommand, Role>()
            .AfterMap(
                (src, dest) =>
                {
                    dest.Name = src.Name.ToSnakeCase().ToUpper();
                }
            );

        CreateMap<RoleClaimModel, RoleClaim>()
            .ForMember(
                dest => dest.Id,
                opt =>
                {
                    opt.Ignore();
                }
            )
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
        CreateMap<Role, CreateRoleResponse>();
    }
}
