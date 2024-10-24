using Domain.Aggregates.Regions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations.Regions;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<District> builder
    )
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Communes).WithOne().HasForeignKey(x => x.DistrictId);
    }
}
