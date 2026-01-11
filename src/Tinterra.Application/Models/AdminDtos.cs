using Tinterra.Domain.Enums;

namespace Tinterra.Application.Models;

public record AllowedTenantDto(Guid TenantId, string DisplayName, bool IsEnabled, DateTime CreatedAtUtc);

public record PermissionDto(string Name, string Description, bool IsSystem);

public record PermissionBundleDto(string Name, string Description, bool IsSystem);

public record BundlePermissionDto(string BundleName, string PermissionName);

public record TenantGroupBundleMappingDto(Guid Id, Guid TenantId, string GroupObjectId, string BundleName);

public record UserProfileDto(string ObjectId, int ApprovalLevel, IReadOnlyCollection<ConfigurationRegion> AllowedRegions);
