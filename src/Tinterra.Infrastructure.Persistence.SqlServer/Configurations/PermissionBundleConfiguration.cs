using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Configurations;

public class PermissionBundleConfiguration : IEntityTypeConfiguration<PermissionBundle>
{
    public void Configure(EntityTypeBuilder<PermissionBundle> builder)
    {
        builder.HasKey(x => x.Name);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
