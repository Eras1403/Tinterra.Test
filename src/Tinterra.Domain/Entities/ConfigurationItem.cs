using Tinterra.Domain.Enums;

namespace Tinterra.Domain.Entities;

public class ConfigurationItem
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public ConfigurationEnvironment Environment { get; set; }
    public ConfigurationRegion Region { get; set; }
    public string OwnerObjectId { get; set; } = string.Empty;
    public ConfigurationClassification Classification { get; set; }
    public ConfigurationStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
