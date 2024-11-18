using Domain.Specs;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByIdSpecification : Specification<User>
{
    public GetUserByIdSpecification(Ulid id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.UserRoles)!
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x!.RoleClaims)
            .Include(x => x.UserClaims)
            .AsSplitQuery()
            .AsNoTracking();
    }
}
