namespace Tinterra.Domain.Entities;

public class TenantGroupBundleMapping
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string GroupObjectId { get; set; } = string.Empty;
    public string BundleName { get; set; } = string.Empty;
}
