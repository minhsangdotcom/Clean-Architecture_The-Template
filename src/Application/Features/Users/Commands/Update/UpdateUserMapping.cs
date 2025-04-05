using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.Features.Users.Commands.Update;

// public class UpdateUserMapping : Profile
// {
//     public UpdateUserMapping()
//     {
//         CreateMap<UpdateUser, User>()
//             .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
//             .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
//         CreateMap<User, UpdateUserResponse>().IncludeBase<User, UserDetailProjection>();
//     }
// }

public static class UpdateUserMapping
{
    public static User FromUpdateUser(this User user, UpdateUser update)
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
        KindaUserClaimType claimType,
        Ulid userId
    )
    {
        return
        [
            .. userClaims.Select(claim => new UserClaim()
            {
                ClaimType = claim.ClaimType!,
                ClaimValue = claim.ClaimValue!,
                Type = claimType,
                UserId = userId,
            }),
        ];
    }
}
