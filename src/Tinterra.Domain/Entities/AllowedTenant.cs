namespace Tinterra.Domain.Entities;

public class AllowedTenant
{
    public Guid TenantId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
