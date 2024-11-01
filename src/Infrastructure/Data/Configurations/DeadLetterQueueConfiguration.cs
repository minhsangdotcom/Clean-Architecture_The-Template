using Application.Common.Interfaces.Services.DistributedCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DeadLetterQueueConfiguration : IEntityTypeConfiguration<DeadLetterQueue>
{
    public void Configure(EntityTypeBuilder<DeadLetterQueue> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ErrorDetail).HasColumnType("jsonb");
    }
}
