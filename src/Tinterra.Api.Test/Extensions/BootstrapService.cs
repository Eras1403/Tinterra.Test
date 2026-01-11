using Microsoft.EntityFrameworkCore;
using Tinterra.Domain.Entities;
using Tinterra.Infrastructure.Persistence.SqlServer;

namespace Tinterra.Api.Test.Extensions;

public static class BootstrapService
{
    public static async Task EnsureBootstrapAsync(IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SqlServerDb>();
         await db.Database.MigrateAsync(cancellationToken);

        if (await db.PermissionBundles.AnyAsync(cancellationToken))
        {
            return;
        }

        var adminBundle = new PermissionBundle { Name = "Security.Admin", Description = "Bootstrap admin bundle", IsSystem = true };
        var adminPermission = new Permission { Name = "Security.Admin", Description = "Full security administration", IsSystem = true };
        db.PermissionBundles.Add(adminBundle);
        db.Permissions.Add(adminPermission);
        db.BundlePermissions.Add(new BundlePermission { BundleName = adminBundle.Name, PermissionName = adminPermission.Name });

        var bootstrapOid = configuration["BootstrapAdminObjectId"];
        var bootstrapGroup = configuration["BootstrapAdminGroupId"];
        if (!string.IsNullOrWhiteSpace(bootstrapGroup) && Guid.TryParse(bootstrapGroup, out var groupId))
        {
            db.TenantGroupBundleMappings.Add(new TenantGroupBundleMapping
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.Empty,
                GroupObjectId = groupId.ToString(),
                BundleName = adminBundle.Name
            });
        }
        else if (!string.IsNullOrWhiteSpace(bootstrapOid))
        {
            db.TenantGroupBundleMappings.Add(new TenantGroupBundleMapping
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.Empty,
                GroupObjectId = bootstrapOid,
                BundleName = adminBundle.Name
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
