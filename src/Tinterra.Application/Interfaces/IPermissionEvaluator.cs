namespace Tinterra.Application.Interfaces;

public interface IPermissionEvaluator
{
    Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid tenantId, string userObjectId, CancellationToken cancellationToken);
}
