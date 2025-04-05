using Application.Common.Security;
using Application.Features.Common.Mapping.Regions;
using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Districts;
using Application.Features.Regions.Queries.List.Provinces;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using SharedKernel.Models;

namespace Application.Features.Common.Projections.Users;

public class UserProjection : BaseResponse
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public ProvinceProjection? Province { get; set; }

    public DistrictProjection? District { get; set; }

    public CommuneProjection? Commune { get; set; }

    public string? Street { get; set; }

    [File]
    public string? Avatar { get; set; }

    public UserStatus Status { get; set; }

    public virtual void MappingFrom(User user)
    {
        Id = user.Id;
        CreatedAt = user.CreatedAt;
        CreatedBy = user.CreatedBy;
        UpdatedAt = user.UpdatedAt;
        UpdatedBy = user.UpdatedBy;

        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Username;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        DayOfBirth = user.DayOfBirth;
        Gender = user.Gender;
        Avatar = user.Avatar;
        Status = user.Status;
        Street = user.Address?.Street;
        Province = user.Address?.Province?.ToProvinceProjection();
        District = user.Address?.District?.ToDistrictProjection();
        Commune = user.Address?.Commune?.ToCommuneProjection();
    }
}
