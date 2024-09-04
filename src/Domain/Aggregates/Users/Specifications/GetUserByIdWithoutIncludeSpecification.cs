using Domain.Aggregates.Users;
using Domain.Specs;

namespace web.Specification.Specs;

public class GetUserByIdWithoutIncludeSpecification : Specification<User>
{
    public GetUserByIdWithoutIncludeSpecification(Ulid id)
    {
        Query.Where(x => x.Id == id).AsNoTracking();
    }
}