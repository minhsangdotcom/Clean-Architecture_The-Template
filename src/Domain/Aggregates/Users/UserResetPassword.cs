using Domain.Common;

namespace Domain.Aggregates.Users;

public class UserResetPassword : BaseEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTimeOffset Expiry { get; set; }

    public Ulid UserId { get; set; }

    public User? User { get; set; }
}
