using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.UseCases.Roles.Queries.Detail;

public class GetRoleDetailMapping : Profile
{
    public GetRoleDetailMapping()
    {
        CreateMap<Role, RoleDetailResponse>();
    }
}
