using Microsoft.AspNetCore.Http;

namespace Application.Features.Common.Projections.Users;

public class UserModel
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public Ulid ProvinceId { get; set; }

    public Ulid DistrictId { get; set; }

    public Ulid? CommuneId { get; set; }

    public string? Street { get; set; }

    public IFormFile? Avatar { get; set; }
}
