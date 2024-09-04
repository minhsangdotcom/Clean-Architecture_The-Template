using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Identity;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserClaims)
            .HasForeignKey(x => x.UserId);
    }
}