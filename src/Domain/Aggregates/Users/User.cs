using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Ardalis.GuardClauses;
using Contracts.Constants;
using Contracts.Extensions.Collections;
using Contracts.Extensions.Reflections;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Events;
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

    // default user claim are ready to update into db
    [NotMapped]
    public IReadOnlyCollection<UserClaim> DefaultUserClaimsToUpdates { get; private set; } = [];

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

    public void UpdateAddress(Address address) => Address = address;

    public void UpdateDefaultUserClaims()
    {
        Emit(new UpdateDefaultUserClaimEvent() { User = this });
    }

    public void CreateDefaultUserClaims()
    {
        ApplyCreateDefaultUserClaim();
    }

    private List<UserClaim> GetUserClaims(bool isCreated = false) =>
        [
            new()
            {
                ClaimType = ClaimTypes.GivenName,
                ClaimValue = this.GetValue(x => x.FirstName!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.FamilyName,
                ClaimValue = this.GetValue(x => x.LastName!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.PreferredUsername,
                ClaimValue = this.GetValue(x => x.UserName!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.BirthDate,
                ClaimValue = this.GetValue(x => x.DayOfBirth!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.Address,
                ClaimValue = this.GetValue(x => x.Address!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.Picture,
                ClaimValue = this.GetValue(x => x.Avatar!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.Gender,
                ClaimValue = this.GetValue(x => x.Gender!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
            new()
            {
                ClaimType = ClaimTypes.Email,
                ClaimValue = this.GetValue(x => x.Email!),
                UserId = isCreated ? Ulid.Empty : Id,
            },
        ];

    private void ApplyUpdateDefaultUserClaim()
    {
        if (UserClaims == null || UserClaims.Count <= 0)
        {
            return;
        }

        Span<UserClaim> currentUserClaims = CollectionsMarshal.AsSpan(
            (UserClaims as List<UserClaim>)!.FindAll(x => x.Type == KindaUserClaimType.Default)
        );
        // default claims with claim type are unique but it's difference with custom claims
        IDictionary<string, string> userClaims = GetUserClaims()
            .ToDictionary(x => x.ClaimType, x => x.ClaimValue);

        for (int i = 0; i < currentUserClaims.Length; i++)
        {
            UserClaim currentUserClaim = currentUserClaims[i];

            string? value = userClaims[currentUserClaim.ClaimType];

            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            currentUserClaim.ClaimValue = value;
        }

        DefaultUserClaimsToUpdates = currentUserClaims.ToArray();
    }

    private void ApplyCreateDefaultUserClaim()
    {
        if (UserClaims != null && UserClaims.Count > 0)
        {
            return;
        }
        UserClaims = GetUserClaims(true);
    }

    protected override bool TryApplyDomainEvent(INotification domainEvent)
    {
        switch (domainEvent)
        {
            case UpdateDefaultUserClaimEvent:
                ApplyUpdateDefaultUserClaim();
                return true;
            default:
                return false;
        }
    }
}
