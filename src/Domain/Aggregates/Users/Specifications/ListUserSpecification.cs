using Domain.Specs;

namespace Domain.Aggregates.Users.Specifications;

public class ListUserSpecification : Specification<User>
{
    public ListUserSpecification()
    {
        Query
            .Include(x => x.Address!.Province)
            .Include(x => x.Address!.District)
            .Include(x => x.Address!.Commune)
            .AsNoTracking()
            .AsSplitQuery();
        string query = SpecificationEvaluator<User>.SpecStringQuery(this);
        Query.EnableCache(query);
    }
}
