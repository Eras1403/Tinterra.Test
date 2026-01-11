using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class UserAllowedRegionConfiguration : IEntityTypeConfiguration<UserAllowedRegion>
{
    public void Configure(EntityTypeBuilder<UserAllowedRegion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserObjectId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Region).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.HasIndex(x => new { x.UserObjectId, x.Region }).IsUnique();
    }
}
