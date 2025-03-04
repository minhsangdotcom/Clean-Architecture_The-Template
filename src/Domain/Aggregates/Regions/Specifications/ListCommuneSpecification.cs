using SharedKernel.Common.Specs;

namespace Domain.Aggregates.Regions.Specifications;

public class ListCommuneSpecification : Specification<Commune>
{
    public ListCommuneSpecification()
    {
        Query.AsNoTracking();
    }
}
