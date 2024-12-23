using Domain.Common.Specs;

namespace Domain.Aggregates.Regions.Specifications;

public class ListProvinceSpecification : Specification<Province>
{
    public ListProvinceSpecification()
    {
        Query.AsNoTracking();
    }
}
