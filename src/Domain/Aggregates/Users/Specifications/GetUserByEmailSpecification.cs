using Domain.Specs;

namespace Domain.Aggregates.Users.Specifications;

public class GetUserByEmailSpecification : Specification<User>
{
    public GetUserByEmailSpecification(string email)
    {
        Query.Where(x => x.Email == email)
            .AsNoTracking();
    }
}
