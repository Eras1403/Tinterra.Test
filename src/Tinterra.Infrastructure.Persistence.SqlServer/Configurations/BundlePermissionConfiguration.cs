using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class BundlePermissionConfiguration : IEntityTypeConfiguration<BundlePermission>
{
    public void Configure(EntityTypeBuilder<BundlePermission> builder)
    {
        builder.HasKey(x => new { x.BundleName, x.PermissionName });
        builder.Property(x => x.BundleName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PermissionName).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.PermissionName);
    }
}
