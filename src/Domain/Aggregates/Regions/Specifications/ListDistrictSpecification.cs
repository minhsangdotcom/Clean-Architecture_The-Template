using Specification;

namespace Domain.Aggregates.Regions.Specifications;

public class ListDistrictSpecification : Specification<District>
{
    public ListDistrictSpecification()
    {
        Query.AsNoTracking().AsSplitQuery();
    }
}
