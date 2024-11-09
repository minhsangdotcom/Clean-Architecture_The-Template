using Domain.Aggregates.Regions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations.Regions;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Province> builder
    )
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Districts).WithOne().HasForeignKey(x => x.ProvinceId);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedAt);
        builder.Ignore(x => x.UpdatedBy);
    }
}
