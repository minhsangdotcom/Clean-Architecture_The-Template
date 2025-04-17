using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Update;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.SubcutaneousTests.Extensions;

public static class UserMappingExtension
{
    public static UpdateUserCommand ToUpdateUserCommand(User user) =>
        new()
        {
            UserId = user.Id.ToString(),
            User = new UpdateUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DayOfBirth = user.DayOfBirth,
                PhoneNumber = user.PhoneNumber,
                ProvinceId = user.Address!.Province!.Id,
                DistrictId = user.Address!.District!.Id,
                CommuneId = user.Address!.Commune!.Id,
                Street = user.Address.Street,
                Roles = [.. user.UserRoles!.Select(x => x.RoleId)],
                UserClaims =
                [
                    .. user.UserClaims!.Where(x => x.Type == KindaUserClaimType.Custom)
                        .Select(x => new UserClaimModel()
                        {
                            ClaimType = x.ClaimType,
                            ClaimValue = x.ClaimValue,
                        }),
                ],
            },
        };
}
