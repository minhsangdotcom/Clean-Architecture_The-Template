using System.Linq.Expressions;
using Application.Features.Common.Mapping.Regions;
using Application.Features.Regions.Queries.List.Districts;
using Application.Features.Regions.Queries.List.Provinces;
using Domain.Aggregates.Users;

namespace Application.Features.Users.Queries.List;

public static class ListUserMapping
{
    public static Expression<Func<User, ListUserResponse>> Selector() =>
        user => new ListUserResponse()
        {
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            CreatedBy = user.CreatedBy,
            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy,

            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DayOfBirth = user.DayOfBirth,
            Gender = user.Gender,
            Avatar = user.Avatar,
            Status = user.Status,
            //Street = user.Address!.Street,
            // Province = user.Address!.Province!.ToProvinceProjection(),
            // District = user.Address!.District!.ToDistrictProjection(),
            // Commune = user.Address!.Commune!.ToCommuneProjection(),
            Address = user.Address!.ToString(),
        };
}
