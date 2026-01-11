using Tinterra.Domain.Entities;

namespace Tinterra.Application.Interfaces;

public interface ISecurityAdminRepository
{
    Task<IReadOnlyCollection<Permission>> GetPermissionsAsync(CancellationToken cancellationToken);
    Task AddPermissionAsync(Permission permission, CancellationToken cancellationToken);
    Task UpdatePermissionAsync(Permission permission, CancellationToken cancellationToken);
    Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermissionBundle>> GetBundlesAsync(CancellationToken cancellationToken);
    Task AddBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken);
    Task UpdateBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken);
    Task DeleteBundleAsync(PermissionBundle bundle, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BundlePermission>> GetBundlePermissionsAsync(string bundleName, CancellationToken cancellationToken);
    Task SetBundlePermissionsAsync(string bundleName, IReadOnlyCollection<string> permissionNames, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<TenantGroupBundleMapping>> GetTenantGroupMappingsAsync(Guid tenantId, CancellationToken cancellationToken);
    Task AddTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken);
    Task UpdateTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken);
    Task DeleteTenantGroupMappingAsync(TenantGroupBundleMapping mapping, CancellationToken cancellationToken);
}
