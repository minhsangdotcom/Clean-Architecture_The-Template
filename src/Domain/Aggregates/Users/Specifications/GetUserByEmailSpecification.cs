using Contracts.Extensions;
using Domain.Specs;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByEmailSpecification : Specification<User>
{
    public GetUserByEmailSpecification(string email)
    {
        Query.Where(x => x.Email == email).Include(x => x.UserResetPasswords).AsNoTracking();
        string key = GetUniqueCachedKey(new { email });
        Query.EnableCache(key);
    }
}
