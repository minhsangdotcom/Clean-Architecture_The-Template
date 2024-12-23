using AutoMapper;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Queries.Detail;

public class GetRoleDetailMapping : Profile
{
    public GetRoleDetailMapping()
    {
        CreateMap<Role, RoleDetailResponse>();
    }
}
