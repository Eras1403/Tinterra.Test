using Microsoft.EntityFrameworkCore;
using Tinterra.Application.Interfaces;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Repositories;

public class SecurityAdminRepository : ISecurityAdminRepository
{
    private readonly SqlServerDb _db;

    public SecurityAdminRepository(SqlServerDb db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<Permission>> GetPermissionsAsync(CancellationToken cancellationToken)
        => await _db.Permissions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddPermissionAsync(Permission permission, CancellationToken cancellationToken)
    {
        _db.Permissions.Add(permission);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePermissionAsync(Permission permission, CancellationToken cancellationToken)
    {
        var existing = await _db.Permissions.FirstOrDefaultAsync(x => x.Name == permission.Name, cancellationToken);
        if (existing is null)
        {
            _db.Permissions.Add(permission);
        }
        else
        {
            existing.Description = permission.Description;
            existing.IsSystem = permission.IsSystem;
            _db.Permissions.Update(existing);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken)
    {
        _db.Permissions.Remove(permission);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionBundle>> GetBundlesAsync(CancellationToken cancellationToken)
        => await _db.PermissionBundles.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken)
    {
        _db.PermissionBundles.Add(bundle);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken)
    {
        var existing = await _db.PermissionBundles.FirstOrDefaultAsync(x => x.Name == bundle.Name, cancellationToken);
        if (existing is null)
        {
            _db.PermissionBundles.Add(bundle);
        }
        else
        {
            existing.Description = bundle.Description;
            existing.IsSystem = bundle.IsSystem;
            _db.PermissionBundles.Update(existing);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken)
    {
        _db.PermissionBundles.Remove(bundle);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<BundlePermission>> GetBundlePermissionsAsync(string bundleName, CancellationToken cancellationToken)
        => await _db.BundlePermissions.AsNoTracking().Where(x => x.BundleName == bundleName).ToListAsync(cancellationToken);

    public async Task SetBundlePermissionsAsync(string bundleName, IReadOnlyCollection<string> permissionNames, CancellationToken cancellationToken)
    {
        var existing = await _db.BundlePermissions.Where(x => x.BundleName == bundleName).ToListAsync(cancellationToken);
        _db.BundlePermissions.RemoveRange(existing);

        foreach (var permissionName in permissionNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            _db.BundlePermissions.Add(new BundlePermission { BundleName = bundleName, PermissionName = permissionName });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TenantGroupBundleMapping>> GetTenantGroupMappingsAsync(Guid tenantId, CancellationToken cancellationToken)
        => await _db.TenantGroupBundleMappings.AsNoTracking().Where(x => x.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken)
    {
        _db.TenantGroupBundleMappings.Add(mapping);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken)
    {
        _db.TenantGroupBundleMappings.Update(mapping);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken)
    {
        _db.TenantGroupBundleMappings.Remove(mapping);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
