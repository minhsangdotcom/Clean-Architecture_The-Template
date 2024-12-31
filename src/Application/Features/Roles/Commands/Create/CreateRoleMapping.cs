using Application.Features.Common.Projections.Roles;
using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleMapping : Profile
{
    public CreateRoleMapping()
    {
        CreateMap<CreateRoleCommand, Role>().IncludeBase<RoleModel, Role>();
        CreateMap<Role, CreateRoleResponse>();
    }
}
