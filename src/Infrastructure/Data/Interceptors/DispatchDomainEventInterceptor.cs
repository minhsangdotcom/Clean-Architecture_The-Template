using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.Interceptors;

public class DispatchDomainEventInterceptor(IServiceScopeFactory serviceScopeFactory) : SaveChangesInterceptor
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

        IEnumerable<AggregateRoot> entities = context
            .ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.UncommittedEvents.Count != 0)
            .Select(e => e.Entity);

        List<INotification> domainEvents = entities.SelectMany(e => e.UncommittedEvents).ToList();
        entities.ToList().ForEach(e => e.DequeueUncommittedEvents());

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPublisher mediator = scope.ServiceProvider.GetRequiredService<IPublisher>();
        foreach (INotification domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
