using Application.Common.Security;
using Application.Features.Common.Projections.Regions;
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
}
