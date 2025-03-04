using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Specs;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByUsernameSpecification : Specification<User>
{
    public GetUserByUsernameSpecification(string username)
    {
        Query.Where(x => EF.Functions.Like(x.Username, username)).AsNoTracking();
    }
}
