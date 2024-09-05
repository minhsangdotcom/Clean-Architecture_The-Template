using Domain.Aggregates.Users;
using Domain.Specs;

namespace Domain.Aggregates.Users.Specifications;


public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string userName)
    {
        Query.Where(x => x.UserName == userName).AsNoTracking();
    }
}