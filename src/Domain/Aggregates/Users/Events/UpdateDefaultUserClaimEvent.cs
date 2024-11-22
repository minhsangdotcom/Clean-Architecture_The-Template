using Mediator;

namespace Domain.Aggregates.Users.Events;

public class UpdateDefaultUserClaimEvent : INotification
{
    public User? User { get; set; }
}
