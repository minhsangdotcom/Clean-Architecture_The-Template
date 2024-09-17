using Domain.Specs;
using Microsoft.EntityFrameworkCore;

namespace Domain.Aggregates.Users.Specifications;


public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string userName)
    {
        Query.Where(x => EF.Functions.ILike(x.UserName, userName)).AsNoTracking();
    }
}