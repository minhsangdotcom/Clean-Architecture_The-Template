using Domain.Aggregates.Users.Enums;
namespace Application.UseCases.Projections.Users;

public class UserProjection
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public UserStatus Status { get; set; }
}