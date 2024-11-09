using Domain.Aggregates.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Regions;

public class CommuneConfiguration : IEntityTypeConfiguration<Commune>
{
    public void Configure(EntityTypeBuilder<Commune> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasOne(x => x.District).WithMany(x => x.Communes).HasForeignKey(x => x.DistrictId);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedAt);
        builder.Ignore(x => x.UpdatedBy);
    }
}
