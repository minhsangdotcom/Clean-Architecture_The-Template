using Domain.Aggregates.Roles;

namespace Infrastructure.Services.Identity;

public class RoleClaimComparer : IEqualityComparer<RoleClaim>
{
    public bool Equals(RoleClaim? x, RoleClaim? y)
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

    public int GetHashCode(RoleClaim obj)
    {
        return obj.Id.GetHashCode();
    }
}
