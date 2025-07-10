using Specification;
using Specification.Builders;

namespace Domain.Aggregates.Users.Specifications;

public class ListUserSpecification : Specification<User>
{
    public ListUserSpecification()
    {
        Query.Include(x => x.Address).AsNoTracking();
        string key = GetUniqueCachedKey();
        Query.EnableCache(key);
    }
}
