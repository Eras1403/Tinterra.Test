using Microsoft.AspNetCore.Authorization;
using Tinterra.Application.Interfaces;

namespace Tinterra.Api.Test.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionEvaluator _permissionEvaluator;
    private readonly ICurrentUserContext _currentUser;

    public PermissionAuthorizationHandler(IPermissionEvaluator permissionEvaluator, ICurrentUserContext currentUser)
    {
        _permissionEvaluator = permissionEvaluator;
        _currentUser = currentUser;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = await _permissionEvaluator.GetPermissionsAsync(_currentUser.TenantId, _currentUser.ObjectId, CancellationToken.None);
        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
