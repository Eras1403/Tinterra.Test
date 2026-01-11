using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class TenantGroupBundleMappingConfiguration : IEntityTypeConfiguration<TenantGroupBundleMapping>
{
    public void Configure(EntityTypeBuilder<TenantGroupBundleMapping> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GroupObjectId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.BundleName).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.GroupObjectId });
    }
}
