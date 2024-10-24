using Ardalis.GuardClauses;
using Contracts.Constants;
using Contracts.Extensions.Reflections;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.ValueObjects;
using Domain.Common;
using Mediator;

namespace Domain.Aggregates.Users;

public class User : AggregateRoot
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string UserName { get; private set; }

    public string Password { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public Address? Address { get; private set; }

    public string? Avatar { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;

    public ICollection<UserClaim>? UserClaims { get; set; } = [];

    public ICollection<UserRole>? UserRoles { get; set; } = [];

    public ICollection<UserToken>? UserTokens { get; set; } = [];

    public ICollection<UserResetPassword>? UserResetPasswords { get; set; } = [];

    public User(
        string firstName,
        string lastName,
        string userName,
        string password,
        string email,
        string phoneNumber,
        Address? address = null
    )
    {
        FirstName = Guard.Against.NullOrEmpty(firstName, nameof(FirstName));
        LastName = Guard.Against.Null(lastName, nameof(LastName));
        UserName = Guard.Against.Null(userName, nameof(UserName));
        Password = Guard.Against.Null(password, nameof(Password));
        Email = Guard.Against.Null(email, nameof(Email));
        PhoneNumber = Guard.Against.Null(phoneNumber, nameof(PhoneNumber));
        Address = address;
    }

    private User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
    }

    public void SetPassword(string password) =>
        Password = Guard.Against.NullOrWhiteSpace(password, nameof(password));

    private void UpdateAddress(Address address) => Address = address;

    public void AddUserClaim(IEnumerable<UserClaim> userClaims)
    {
        Guard.Against.NullOrEmpty(userClaims, nameof(userClaims));
        Guard.Against.InvalidInput(
            userClaims,
            nameof(userClaims),
            userClaims =>
                userClaims.DistinctBy(x => new { x.ClaimType, x.ClaimValue }).Count()
                != userClaims.Count(),
            $"{nameof(userClaims)} is duplicated"
        );

        Enumerable.ToList(UserClaims!).AddRange(userClaims);
    }

    public IEnumerable<UserClaimType> GetUserClaims() =>
        [
            new()
            {
                ClaimType = ClaimTypes.GivenName,
                ClaimValue = this.GetValue(x => x.FirstName!),
            },
            new()
            {
                ClaimType = ClaimTypes.FamilyName,
                ClaimValue = this.GetValue(x => x.LastName!),
            },
            new()
            {
                ClaimType = ClaimTypes.PreferredUsername,
                ClaimValue = this.GetValue(x => x.UserName!),
            },
            new()
            {
                ClaimType = ClaimTypes.BirthDate,
                ClaimValue = this.GetValue(x => x.DayOfBirth!),
            },
            new() { ClaimType = ClaimTypes.Address, ClaimValue = this.GetValue(x => x.Address!) },
            new() { ClaimType = ClaimTypes.Picture, ClaimValue = this.GetValue(x => x.Avatar!) },
            new() { ClaimType = ClaimTypes.Gender, ClaimValue = this.GetValue(x => x.Gender!) },
            new() { ClaimType = ClaimTypes.Email, ClaimValue = this.GetValue(x => x.Email!) },
        ];

    protected override bool TryApplyDomainEvent(INotification domainEvent)
    {
        return false;
    }
}
