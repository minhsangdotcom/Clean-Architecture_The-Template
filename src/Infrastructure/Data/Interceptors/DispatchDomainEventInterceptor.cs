using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

public class DispatchDomainEventInterceptor(IPublisher mediator) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null)
            return;

        IEnumerable<BaseEntity> entities = context
            .ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.UncommittedEvents.Count != 0)
            .Select(e => e.Entity);

        List<INotification> domainEvents = entities.SelectMany(e => e.UncommittedEvents).ToList();

        entities.ToList().ForEach(e => e.DequeueUncommittedEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
