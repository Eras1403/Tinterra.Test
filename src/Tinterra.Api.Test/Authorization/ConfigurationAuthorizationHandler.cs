using Microsoft.AspNetCore.Authorization;
using Tinterra.Application.Interfaces;
using Tinterra.Domain.Entities;
using Tinterra.Domain.Enums;

namespace Tinterra.Api.Test.Authorization;

public class ConfigurationAuthorizationHandler : AuthorizationHandler<ConfigurationAuthorizationRequirement, ConfigurationItem>
{
    private const string AdminPermission = "Security.Admin";
    private const string PublishPermission = "Config.Publish";

    private readonly ICurrentUserContext _currentUser;
    private readonly IPermissionEvaluator _permissionEvaluator;
    private readonly IUserProfileRepository _userProfileRepository;

    public ConfigurationAuthorizationHandler(
        ICurrentUserContext currentUser,
        IPermissionEvaluator permissionEvaluator,
        IUserProfileRepository userProfileRepository)
    {
        _currentUser = currentUser;
        _permissionEvaluator = permissionEvaluator;
        _userProfileRepository = userProfileRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ConfigurationAuthorizationRequirement requirement,
        ConfigurationItem resource)
    {
        var permissions = await _permissionEvaluator.GetPermissionsAsync(_currentUser.TenantId, _currentUser.ObjectId, CancellationToken.None);
        var isAdmin = permissions.Contains(AdminPermission, StringComparer.OrdinalIgnoreCase);

        switch (requirement.Action)
        {
            case ConfigurationAction.View:
                var profile = await _userProfileRepository.GetByObjectIdAsync(_currentUser.ObjectId, CancellationToken.None);
                var allowedRegions = profile?.AllowedRegions.Select(r => r.Region).ToHashSet() ?? [];
                if (allowedRegions.Contains(resource.Region) || resource.OwnerObjectId == _currentUser.ObjectId || resource.Classification != ConfigurationClassification.Confidential)
                {
                    context.Succeed(requirement);
                }
                break;
            case ConfigurationAction.Edit:
            case ConfigurationAction.Delete:
                if (resource.Status == ConfigurationStatus.Draft && (resource.OwnerObjectId == _currentUser.ObjectId || isAdmin))
                {
                    context.Succeed(requirement);
                }
                break;
            case ConfigurationAction.Publish:
                if (resource.Status == ConfigurationStatus.Draft && permissions.Contains(PublishPermission, StringComparer.OrdinalIgnoreCase))
                {
                    if (resource.Environment == ConfigurationEnvironment.Prod)
                    {
                        var approvalLevel = (await _userProfileRepository.GetByObjectIdAsync(_currentUser.ObjectId, CancellationToken.None))?.ApprovalLevel ?? 0;
                        if (approvalLevel >= 2)
                        {
                            context.Succeed(requirement);
                        }
                    }
                    else
                    {
                        context.Succeed(requirement);
                    }
                }
                break;
        }
    }
}
