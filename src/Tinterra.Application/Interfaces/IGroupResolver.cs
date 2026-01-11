namespace Tinterra.Application.Interfaces;

public interface IGroupResolver
{
    Task<IReadOnlyCollection<string>> GetGroupObjectIdsAsync(string userObjectId, CancellationToken cancellationToken);
}
