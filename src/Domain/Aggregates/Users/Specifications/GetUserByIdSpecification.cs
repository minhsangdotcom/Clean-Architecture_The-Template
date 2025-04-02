using Specification;
using Specification.Builders;

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
            .Include(x => x.Address!.Province)
            .Include(x => x.Address!.District)
            .Include(x => x.Address!.Commune)
            .AsSplitQuery();
    }
}
