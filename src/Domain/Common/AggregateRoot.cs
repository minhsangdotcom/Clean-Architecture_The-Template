using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Mediator;

namespace Domain.Common;

public abstract class AggregateRoot
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public long Version { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonIgnore]
    [NotMapped]
    public IReadOnlyCollection<INotification> UncommittedEvents => uncommittedEvents;

    [JsonIgnore]
    [NotMapped]
    private readonly Queue<INotification> uncommittedEvents = [];

    public INotification[] DequeueUncommittedEvents()
    {
        var dequeuedEvents = uncommittedEvents.ToArray();

        uncommittedEvents.Clear();

        return dequeuedEvents;
    }

    protected void Emit(INotification domainEvent)
    {
        if (TryApplyDomainEvent(domainEvent))
        {
            uncommittedEvents.Enqueue(domainEvent);
        }
    }

    protected abstract bool TryApplyDomainEvent(INotification domainEvent);
}
