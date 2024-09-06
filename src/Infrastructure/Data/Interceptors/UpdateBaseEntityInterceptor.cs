using Application.Common.Interfaces.Services;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

public class UpdateBaseEntityInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    private const string ANONYMOUS_CREATED_BY = "SYSTEM";

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        var entities = context.ChangeTracker.Entries<BaseEntity>().ToList();

        foreach (EntityEntry<BaseEntity> entry in entities)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(BaseEntity.CreatedBy)).CurrentValue = currentUser.Id?.ToString() ?? ANONYMOUS_CREATED_BY;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(BaseEntity.UpdatedBy)).CurrentValue = currentUser.Id?.ToString() ?? ANONYMOUS_CREATED_BY;

                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = currentTime;
            }
        }
    }
}