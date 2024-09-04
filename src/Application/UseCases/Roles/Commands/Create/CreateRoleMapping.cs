using Application.UseCases.Projections.Roles;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleMapping : Profile
{
    public CreateRoleMapping()
    {
        CreateMap<CreateRoleCommand, Role>()
        .AfterMap((src,dest) =>
        {
            dest.Name = src.Name!.ToUpper();
        });

        CreateMap<RoleClaimModel, RoleClaim>()
            .ForMember(dest => dest.Id, opt =>
            {
                opt.PreCondition(x => x.Id == Ulid.Empty);
                opt.Ignore();
            });

        CreateMap<RoleClaim, RoleClaimDetailProjection>();
        CreateMap<Role, CreateRoleResponse>();
    }
}