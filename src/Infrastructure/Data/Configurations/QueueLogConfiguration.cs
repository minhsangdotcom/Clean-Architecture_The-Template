using Domain.Aggregates.QueueLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DeadLetterQueueConfiguration : IEntityTypeConfiguration<QueueLog>
{
    public void Configure(EntityTypeBuilder<QueueLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ErrorDetail).HasColumnType("jsonb");
        builder.Property(x => x.Request).HasColumnType("jsonb");
    }
}
