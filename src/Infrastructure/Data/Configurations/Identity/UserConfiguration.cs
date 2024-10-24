using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.DayOfBirth).HasColumnType("date");
        builder.Property(x => x.UserName).HasColumnType("citext");
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.Property(x => x.Email).HasColumnType("citext");
        builder.HasIndex(x => x.Email).IsUnique();

        builder.OwnsOne(
            x => x.Address,
            address =>
            {
                address.Property(x => x.Street).IsRequired();

                address.HasOne(x => x.Province);
                address.HasOne(x => x.District);
                address.HasOne(x => x.Commune);
            }
        );
    }
}
