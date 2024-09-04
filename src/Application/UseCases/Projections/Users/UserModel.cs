using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Projections.Users;

public class UserModel
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public string? Address { get; set; }

    public IFormFile? Avatar { get; set; }
}