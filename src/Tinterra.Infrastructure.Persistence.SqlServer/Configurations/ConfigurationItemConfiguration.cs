using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class ConfigurationItemConfiguration : IEntityTypeConfiguration<ConfigurationItem>
{
    public void Configure(EntityTypeBuilder<ConfigurationItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.OwnerObjectId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Environment).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Region).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(x => x.Classification).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.HasIndex(x => new { x.Key, x.Environment, x.Region }).IsUnique();
    }
}
