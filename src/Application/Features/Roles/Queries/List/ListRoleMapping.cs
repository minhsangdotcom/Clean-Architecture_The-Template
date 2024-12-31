using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Queries.List;

public class ListRoleMapping : Profile
{
    public ListRoleMapping()
    {
        CreateMap<Role, ListRoleResponse>();
    }
}
