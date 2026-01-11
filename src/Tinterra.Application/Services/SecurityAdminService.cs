using Tinterra.Application.Interfaces;
using Tinterra.Application.Models;
using Tinterra.Domain.Entities;
using Tinterra.Domain.Enums;

namespace Tinterra.Application.Services;

public class SecurityAdminService
{
    private readonly IAllowedTenantRepository _tenantRepository;
    private readonly ISecurityAdminRepository _securityRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public SecurityAdminService(
        IAllowedTenantRepository tenantRepository,
        ISecurityAdminRepository securityRepository,
        IUserProfileRepository userProfileRepository)
    {
        _tenantRepository = tenantRepository;
        _securityRepository = securityRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<Result<IReadOnlyCollection<AllowedTenantDto>>> GetTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<AllowedTenantDto>>.Success(tenants.Select(ToDto).ToList());
    }

    public async Task<Result<AllowedTenantDto>> UpsertTenantAsync(AllowedTenantDto dto, CancellationToken cancellationToken)
    {
        var existing = await _tenantRepository.GetByIdAsync(dto.TenantId, cancellationToken);
        if (existing is null)
        {
            existing = new AllowedTenant
            {
                TenantId = dto.TenantId,
                CreatedAtUtc = DateTime.UtcNow
            };
            await _tenantRepository.AddAsync(existing, cancellationToken);
        }

        existing.DisplayName = dto.DisplayName;
        existing.IsEnabled = dto.IsEnabled;
        await _tenantRepository.UpdateAsync(existing, cancellationToken);
        return Result<AllowedTenantDto>.Success(ToDto(existing));
    }

    public async Task<Result<IReadOnlyCollection<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        var permissions = await _securityRepository.GetPermissionsAsync(cancellationToken);
        return Result<IReadOnlyCollection<PermissionDto>>.Success(permissions.Select(p => new PermissionDto(p.Name, p.Description, p.IsSystem)).ToList());
    }

    public async Task<Result<PermissionDto>> UpsertPermissionAsync(PermissionDto dto, CancellationToken cancellationToken)
    {
        var permission = new Permission { Name = dto.Name, Description = dto.Description, IsSystem = dto.IsSystem };
        await _securityRepository.UpdatePermissionAsync(permission, cancellationToken);
        return Result<PermissionDto>.Success(dto);
    }

    public async Task<Result<IReadOnlyCollection<PermissionBundleDto>>> GetBundlesAsync(CancellationToken cancellationToken)
    {
        var bundles = await _securityRepository.GetBundlesAsync(cancellationToken);
        return Result<IReadOnlyCollection<PermissionBundleDto>>.Success(bundles.Select(b => new PermissionBundleDto(b.Name, b.Description, b.IsSystem)).ToList());
    }

    public async Task<Result<PermissionBundleDto>> UpsertBundleAsync(PermissionBundleDto dto, CancellationToken cancellationToken)
    {
        var bundle = new PermissionBundle { Name = dto.Name, Description = dto.Description, IsSystem = dto.IsSystem };
        await _securityRepository.UpdateBundleAsync(bundle, cancellationToken);
        return Result<PermissionBundleDto>.Success(dto);
    }

    public async Task<Result<IReadOnlyCollection<BundlePermissionDto>>> GetBundlePermissionsAsync(string bundleName, CancellationToken cancellationToken)
    {
        var permissions = await _securityRepository.GetBundlePermissionsAsync(bundleName, cancellationToken);
        return Result<IReadOnlyCollection<BundlePermissionDto>>.Success(permissions.Select(p => new BundlePermissionDto(p.BundleName, p.PermissionName)).ToList());
    }

    public async Task<Result<bool>> SetBundlePermissionsAsync(string bundleName, IReadOnlyCollection<string> permissionNames, CancellationToken cancellationToken)
    {
        await _securityRepository.SetBundlePermissionsAsync(bundleName, permissionNames, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IReadOnlyCollection<TenantGroupBundleMappingDto>>> GetTenantGroupMappingsAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var mappings = await _securityRepository.GetTenantGroupMappingsAsync(tenantId, cancellationToken);
        return Result<IReadOnlyCollection<TenantGroupBundleMappingDto>>.Success(mappings.Select(ToDto).ToList());
    }

    public async Task<Result<TenantGroupBundleMappingDto>> UpsertMappingAsync(TenantGroupBundleMappingDto dto, CancellationToken cancellationToken)
    {
        var mapping = new TenantGroupBundleMapping
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            TenantId = dto.TenantId,
            GroupObjectId = dto.GroupObjectId,
            BundleName = dto.BundleName
        };

        if (dto.Id == Guid.Empty)
        {
            await _securityRepository.AddTenantGroupMappingAsync(mapping, cancellationToken);
        }
        else
        {
            await _securityRepository.UpdateTenantGroupMappingAsync(mapping, cancellationToken);
        }

        return Result<TenantGroupBundleMappingDto>.Success(ToDto(mapping));
    }

    public async Task<Result<bool>> DeleteMappingAsync(TenantGroupBundleMappingDto dto, CancellationToken cancellationToken)
    {
        var mapping = new TenantGroupBundleMapping
        {
            Id = dto.Id,
            TenantId = dto.TenantId,
            GroupObjectId = dto.GroupObjectId,
            BundleName = dto.BundleName
        };

        await _securityRepository.DeleteTenantGroupMappingAsync(mapping, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<UserProfileDto?>> GetUserProfileAsync(string objectId, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByObjectIdAsync(objectId, cancellationToken);
        if (profile is null)
        {
            return Result<UserProfileDto?>.Success(null);
        }

        return Result<UserProfileDto?>.Success(new UserProfileDto(profile.ObjectId, profile.ApprovalLevel, profile.AllowedRegions.Select(r => r.Region).ToList()));
    }

    public async Task<Result<UserProfileDto>> UpsertUserProfileAsync(UserProfileDto dto, CancellationToken cancellationToken)
    {
        var profile = new UserProfile
        {
            ObjectId = dto.ObjectId,
            ApprovalLevel = dto.ApprovalLevel,
            AllowedRegions = dto.AllowedRegions.Select(region => new UserAllowedRegion
            {
                Id = Guid.NewGuid(),
                UserObjectId = dto.ObjectId,
                Region = region
            }).ToList()
        };

        await _userProfileRepository.UpsertAsync(profile, cancellationToken);
        return Result<UserProfileDto>.Success(dto);
    }

    private static AllowedTenantDto ToDto(AllowedTenant tenant)
        => new(tenant.TenantId, tenant.DisplayName, tenant.IsEnabled, tenant.CreatedAtUtc);

    private static TenantGroupBundleMappingDto ToDto(TenantGroupBundleMapping mapping)
        => new(mapping.Id, mapping.TenantId, mapping.GroupObjectId, mapping.BundleName);
}
