using System.Text.Json.Serialization;
using Application.Common.Security;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.ValueObjects;
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

    public string? Address { get; set; }

    [JsonIgnore]
    public Address? UserAddress { get; set; }

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
        UserAddress = user.Address;
        Address = user.Address?.ToString();
    }
}
