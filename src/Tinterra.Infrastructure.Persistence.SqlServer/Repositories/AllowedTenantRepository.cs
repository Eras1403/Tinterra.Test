using Microsoft.EntityFrameworkCore;
using Tinterra.Application.Interfaces;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Repositories;

public class AllowedTenantRepository : IAllowedTenantRepository
{
    private readonly SqlServerDb _db;

    public AllowedTenantRepository(SqlServerDb db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<AllowedTenant>> GetAllAsync(CancellationToken cancellationToken)
        => await _db.AllowedTenants.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<AllowedTenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken)
        => await _db.AllowedTenants.FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

    public async Task AddAsync(AllowedTenant tenant, CancellationToken cancellationToken)
    {
        _db.AllowedTenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AllowedTenant tenant, CancellationToken cancellationToken)
    {
        _db.AllowedTenants.Update(tenant);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
