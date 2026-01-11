using Tinterra.Domain.Entities;

namespace Tinterra.Application.Interfaces;

public interface IAllowedTenantRepository
{
    Task<IReadOnlyCollection<AllowedTenant>> GetAllAsync(CancellationToken cancellationToken);
    Task<AllowedTenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken);
    Task AddAsync(AllowedTenant tenant, CancellationToken cancellationToken);
    Task UpdateAsync(AllowedTenant tenant, CancellationToken cancellationToken);
}
