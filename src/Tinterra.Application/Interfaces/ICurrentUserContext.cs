namespace Tinterra.Application.Interfaces;

public interface ICurrentUserContext
{
    string ObjectId { get; }
    Guid TenantId { get; }
    IReadOnlyCollection<string> Claims { get; }
}
