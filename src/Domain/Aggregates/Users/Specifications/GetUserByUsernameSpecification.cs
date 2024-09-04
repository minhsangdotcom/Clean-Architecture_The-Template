using Domain.Aggregates.Users;
using Domain.Specs;

namespace web.Specification.Specs;


public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string userName)
    {
        Query.Where(x => x.UserName == userName).AsNoTracking();
    }
}