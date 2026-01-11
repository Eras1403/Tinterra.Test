using Microsoft.EntityFrameworkCore;
using Tinterra.Application.Interfaces;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly SqlServerDb _db;

    public ConfigurationRepository(SqlServerDb db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<ConfigurationItem>> GetAllAsync(CancellationToken cancellationToken)
        => await _db.ConfigurationItems.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<ConfigurationItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _db.ConfigurationItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(ConfigurationItem item, CancellationToken cancellationToken)
    {
        _db.ConfigurationItems.Add(item);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ConfigurationItem item, CancellationToken cancellationToken)
    {
        _db.ConfigurationItems.Update(item);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ConfigurationItem item, CancellationToken cancellationToken)
    {
        _db.ConfigurationItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
