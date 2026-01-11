using Microsoft.EntityFrameworkCore;
using Tinterra.Application.Interfaces;
using Tinterra.Domain.Entities;

namespace Tinterra.Infrastructure.Persistence.SqlServer.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly SqlServerDb _db;

    public UserProfileRepository(SqlServerDb db)
    {
        _db = db;
    }

    public async Task<UserProfile?> GetByObjectIdAsync(string objectId, CancellationToken cancellationToken)
    {
        return await _db.UserProfiles
            .Include(x => x.AllowedRegions)
            .FirstOrDefaultAsync(x => x.ObjectId == objectId, cancellationToken);
    }

    public async Task UpsertAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var existing = await _db.UserProfiles.Include(x => x.AllowedRegions)
            .FirstOrDefaultAsync(x => x.ObjectId == profile.ObjectId, cancellationToken);

        if (existing is null)
        {
            _db.UserProfiles.Add(profile);
        }
        else
        {
            existing.ApprovalLevel = profile.ApprovalLevel;
            _db.UserAllowedRegions.RemoveRange(existing.AllowedRegions);
            existing.AllowedRegions = profile.AllowedRegions;
            _db.UserProfiles.Update(existing);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
