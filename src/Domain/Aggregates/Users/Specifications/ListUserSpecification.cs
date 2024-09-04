using Domain.Aggregates.Users;
using Domain.Specs;

namespace web.Specification.Specs;

public class ListUserSpecification : Specification<User>
{
    public ListUserSpecification()
    {
        Query.AsNoTracking();
    }
}