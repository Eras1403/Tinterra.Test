using Tinterra.Domain.Entities;

namespace Tinterra.Application.Interfaces;

public interface IConfigurationRepository
{
    Task<IReadOnlyCollection<ConfigurationItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<ConfigurationItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ConfigurationItem item, CancellationToken cancellationToken);
    Task UpdateAsync(ConfigurationItem item, CancellationToken cancellationToken);
    Task DeleteAsync(ConfigurationItem item, CancellationToken cancellationToken);
}
