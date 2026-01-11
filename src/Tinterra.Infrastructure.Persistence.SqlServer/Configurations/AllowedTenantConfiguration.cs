using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class AllowedTenantConfiguration : IEntityTypeConfiguration<AllowedTenant>
{
    public void Configure(EntityTypeBuilder<AllowedTenant> builder)
    {
        builder.HasKey(x => x.TenantId);
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.DisplayName);
    }
}
