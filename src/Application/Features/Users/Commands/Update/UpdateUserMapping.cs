using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.Features.Users.Commands.Update;

public static class UpdateUserMapping
{
    public static User FromUpdateUser(this User user, UserUpdateRequest update)
    {
        user.Update(
            update.FirstName,
            update.LastName,
            update.Email,
            update.PhoneNumber,
            update.DayOfBirth
        );
        return user;
    }

    public static UpdateUserResponse ToUpdateUserResponse(this User user)
    {
        var response = new UpdateUserResponse();
        response.MappingFrom(user);

        return response;
    }

    public static List<UserClaim> ToListUserClaim(
        this List<UserClaimModel> userClaims,
        UserClaimType claimType,
        Ulid userId
    )
    {
        return
        [
            .. userClaims.Select(claim =>
            {
                var result = new UserClaim()
                {
                    ClaimType = claim.ClaimType!,
                    ClaimValue = claim.ClaimValue!,
                    Type = claimType,
                    UserId = userId,
                };

                if (claim.Id != null)
                {
                    result.Id = claim.Id.Value;
                }
                
                return result;
            }),
        ];
    }
}
