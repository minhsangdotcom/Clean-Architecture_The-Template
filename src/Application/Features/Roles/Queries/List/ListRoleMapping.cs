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
    public static IEnumerable<ListRoleResponse> ToListRoleResponse(this List<Role> roles)
    {
        return roles.Select(role =>
        {
            ListRoleResponse listRole = new();
            listRole.MappingFrom(role);
            return listRole;
        });
    }
}
