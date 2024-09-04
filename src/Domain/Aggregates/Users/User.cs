using Ardalis.GuardClauses;
using Contracts.Constants;
using Contracts.Extensions.Reflections;
using Domain.Aggregates.Users.Enums;
using Domain.Common;
namespace Domain.Aggregates.Users;

public class User(
    string firstName,
    string lastName,
    string userName,
    string password,
    string email,
    string phoneNumber
) : BaseEntity
{
    public string FirstName { get; private set; } = Guard.Against.NullOrEmpty(firstName, nameof(FirstName));

    public string LastName { get; private set; } = Guard.Against.Null(lastName, nameof(LastName));

    public string UserName { get; private set; } = Guard.Against.Null(userName, nameof(UserName));

    public string Password { get; private set; } = Guard.Against.Null(password, nameof(Password));

    public string Email { get; private set; } = Guard.Against.Null(email, nameof(Email));

    public string PhoneNumber { get; set; } = Guard.Against.Null(phoneNumber, nameof(PhoneNumber));

    public DateTime? DayOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;

    public ICollection<UserClaim>? UserClaims { get; set; }

    public ICollection<UserRole>? UserRoles { get; set; }

    public ICollection<UserToken>? UserTokens { get; set; }

    public void SetPassword(string password) =>
        Password = Guard.Against.NullOrWhiteSpace(password,nameof(password));

    public IEnumerable<UserClaimType> GetUserClaims()
    {
        return [
            new()
            {
                ClaimType = ClaimTypes.GivenName,
                ClaimValue = this.GetValue(x => x.FirstName!)
            },
            new()
            {
                ClaimType = ClaimTypes.FamilyName,
                ClaimValue = this.GetValue(x => x.LastName!)
            },
             new()
            {
                ClaimType = ClaimTypes.PreferredUsername,
                ClaimValue = this.GetValue(x => x.UserName!)
            },
            new()
            {
                ClaimType = ClaimTypes.BirthDate,
                ClaimValue = this.GetValue(x => x.DayOfBirth!)
            },
            new()
            {
                ClaimType = ClaimTypes.Address,
                ClaimValue = this.GetValue(x => x.Address!)
            },
            new()
            {
                ClaimType = ClaimTypes.Picture,
                ClaimValue = this.GetValue(x => x.Avatar!)
            },
            new()
            {
                ClaimType = ClaimTypes.Gender,
                ClaimValue = this.GetValue(x => x.Gender!)
            },
            new()
            {
                ClaimType = ClaimTypes.Email,
                ClaimValue = this.GetValue(x => x.Email!)
            },
        ];
    }
}