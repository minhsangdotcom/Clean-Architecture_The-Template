using Domain.Aggregates.Users;

namespace Infrastructure.Services.Identity;

public class UserClaimComparer : IEqualityComparer<UserClaim>
{
    public bool Equals(UserClaim? x, UserClaim? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }
        return x.Id == y.Id;
    }

    public int GetHashCode(UserClaim obj)
    {
        return obj.Id.GetHashCode();
    }
}
