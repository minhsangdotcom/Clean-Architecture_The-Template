using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Roles.Queries.List;

public class ListRoleMapping : Profile
{
    public ListRoleMapping()
    {
        CreateMap<Role, ListRoleResponse>();
    }
}