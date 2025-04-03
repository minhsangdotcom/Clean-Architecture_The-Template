using Application.Features.Common.Mapping.Roles;
using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Queries.Detail;

// public class GetRoleDetailMapping : Profile
// {
//     public GetRoleDetailMapping()
//     {
//         CreateMap<Role, RoleDetailResponse>();
//     }
// }

public static class GetRoleDetailMapping
{
    public static RoleDetailResponse ToRoleDetailResponse(this Role role)
    {
        return new()
        {
            Id = role.Id,
            Name = role.Name,
            Guard = role.Guard,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            RoleClaims = role.RoleClaims?.ToListRoleClaimDetailProjection(),
        };
    }
}
