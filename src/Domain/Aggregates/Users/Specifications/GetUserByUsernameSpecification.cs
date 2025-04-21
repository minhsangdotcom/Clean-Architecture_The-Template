using Microsoft.EntityFrameworkCore;
using Specification;
using Specification.Builders;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string username)
    {
        Query.Where(x => x.Username == username).AsNoTracking();
    }
}
