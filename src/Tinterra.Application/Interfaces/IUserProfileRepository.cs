using Tinterra.Domain.Entities;

namespace Tinterra.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByObjectIdAsync(string objectId, CancellationToken cancellationToken);
    Task UpsertAsync(UserProfile profile, CancellationToken cancellationToken);
}
