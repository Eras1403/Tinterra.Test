using Microsoft.EntityFrameworkCore;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer;

public class SqlServerDb : DbContext
{
    public SqlServerDb(DbContextOptions<SqlServerDb> options) : base(options)
    {
    }

    public DbSet<ConfigurationItem> ConfigurationItems => Set<ConfigurationItem>();
    public DbSet<AllowedTenant> AllowedTenants => Set<AllowedTenant>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<PermissionBundle> PermissionBundles => Set<PermissionBundle>();
    public DbSet<BundlePermission> BundlePermissions => Set<BundlePermission>();
    public DbSet<TenantGroupBundleMapping> TenantGroupBundleMappings => Set<TenantGroupBundleMapping>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserAllowedRegion> UserAllowedRegions => Set<UserAllowedRegion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlServerDb).Assembly);
    }
}
