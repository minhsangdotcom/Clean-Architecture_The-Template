using System.Runtime.InteropServices;
using Contracts.Extensions.Collections;
using Domain.Aggregates.Users;
using Domain.Common;

namespace Domain.Aggregates.Roles;

public class RoleClaim : DefaultEntity
{
    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public Role? Role { get; set; }

    public Ulid RoleId { get; set; }

    public ICollection<UserClaim>? UserClaims { get; set; } = [];

    public List<UserClaim> UpdateUserClaim()
    {
        if (UserClaims == null || UserClaims.Count <= 0)
        {
            return [];
        }

        List<UserClaim>? userClaims = UserClaims.CastToList();
        Span<UserClaim> spans = CollectionsMarshal.AsSpan(userClaims);

        for (int i = 0; i < spans.Length; i++)
        {
            spans[i].ClaimValue = ClaimValue;
        }

        return userClaims!;
    }
}
