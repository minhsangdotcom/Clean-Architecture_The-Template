using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Profiles;
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
            UpdateData = new UserUpdateRequest()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DayOfBirth = user.DayOfBirth,
                PhoneNumber = user.PhoneNumber,
                ProvinceId = user.Address!.ProvinceId,
                DistrictId = user.Address!.DistrictId,
                CommuneId = user.Address!.CommuneId,
                Street = user.Address.Street,
                Roles = [.. user.UserRoles!.Select(x => x.RoleId)],
                UserClaims =
                [
                    .. user.UserClaims!.Where(x => x.Type == UserClaimType.Custom)
                        .Select(x => new UserClaimModel()
                        {
                            ClaimType = x.ClaimType,
                            ClaimValue = x.ClaimValue,
                        }),
                ],
            },
        };

    public static UpdateUserProfileCommand ToUpdateUserProfileCommand(User user) =>
        new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DayOfBirth = user.DayOfBirth,
            PhoneNumber = user.PhoneNumber,
            ProvinceId = user.Address!.ProvinceId,
            DistrictId = user.Address!.DistrictId,
            CommuneId = user.Address!.CommuneId,
            Street = user.Address.Street,
        };
}
