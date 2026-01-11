using Microsoft.Extensions.Caching.Memory;
using Tinterra.Application.Interfaces;

namespace Tinterra.Infrastructure.Identity.Services;

public class PermissionEvaluator : IPermissionEvaluator
{
    private readonly IGroupResolver _groupResolver;
    private readonly ISecurityAdminRepository _securityRepository;
    private readonly IMemoryCache _cache;

    public PermissionEvaluator(IGroupResolver groupResolver, ISecurityAdminRepository securityRepository, IMemoryCache cache)
    {
        _groupResolver = groupResolver;
        _securityRepository = securityRepository;
        _cache = cache;
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid tenantId, string userObjectId, CancellationToken cancellationToken)
    {
        var cacheKey = $"perm:{tenantId}:{userObjectId}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyCollection<string> cached))
        {
            return cached;
        }

        var groups = await _groupResolver.GetGroupObjectIdsAsync(userObjectId, cancellationToken);
        var mappings = (await _securityRepository.GetTenantGroupMappingsAsync(tenantId, cancellationToken))
            .Concat(await _securityRepository.GetTenantGroupMappingsAsync(Guid.Empty, cancellationToken)).ToList();
        var bundles = mappings.Where(m => groups.Contains(m.GroupObjectId, StringComparer.OrdinalIgnoreCase))
            .Select(m => m.BundleName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var bundle in bundles)
        {
            var bundlePermissions = await _securityRepository.GetBundlePermissionsAsync(bundle, cancellationToken);
            foreach (var permission in bundlePermissions)
            {
                permissions.Add(permission.PermissionName);
            }
        }

        var result = permissions.ToList();
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
