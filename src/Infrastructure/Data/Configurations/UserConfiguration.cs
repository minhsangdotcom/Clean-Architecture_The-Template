using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DayOfBirth).HasColumnType("date");
        builder.Property(x => x.UserName).HasColumnType("citext");
        builder.HasIndex(x => x.UserName).IsUnique();
    }
}