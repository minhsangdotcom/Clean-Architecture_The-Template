using Domain.Specs;
using Microsoft.EntityFrameworkCore;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string username)
    {
        Query.Where(x => EF.Functions.ILike(x.Username, username)).AsNoTracking();
    }
}
