using Microsoft.EntityFrameworkCore;
using Specification;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string username)
    {
        Query.Where(x => EF.Functions.Like(x.Username, username)).AsNoTracking();
    }
}
