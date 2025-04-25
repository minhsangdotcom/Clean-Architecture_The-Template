using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;

namespace Application.Features.Common.Mapping.Users;

public static class UserMapping
{
    public static UserClaimDetailProjection ToRoleClaimDetailProjection(this UserClaim userClaim)
    {
        return new()
        {
            ClaimType = userClaim.ClaimType,
            ClaimValue = userClaim.ClaimValue,
            Id = userClaim.Id,
            CreatedAt = userClaim.CreatedAt,
        };
    }

    public static UserProjection ToUserProjection(this User user)
    {
        var response = new UserProjection();
        response.MappingFrom(user);

        return response;
    }
}
