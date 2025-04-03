using Domain.Aggregates.Roles;

namespace Application.Features.Roles.Queries.List;

// public class ListRoleMapping : Profile
// {
//     public ListRoleMapping()
//     {
//         CreateMap<Role, ListRoleResponse>();
//     }
// }

public static class ListRoleMapping
{
    public static ListRoleResponse ToListRoleResponse(this Role role)
    {
        return new()
        {
            Id = role.Id,
            CreatedAt = role.CreatedAt,
            Description = role.Description,
            Name = role.Name,
            Guard = role.Guard,
        };
    }
}
