using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.ObjectId);
        builder.Property(x => x.ObjectId).HasMaxLength(100).IsRequired();
        builder.HasMany(x => x.AllowedRegions)
            .WithOne()
            .HasForeignKey(x => x.UserObjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
