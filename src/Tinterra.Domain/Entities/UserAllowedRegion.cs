using Tinterra.Domain.Enums;

namespace Tinterra.Domain.Entities;

public class UserAllowedRegion
{
    public Guid Id { get; set; }
    public string UserObjectId { get; set; } = string.Empty;
    public ConfigurationRegion Region { get; set; }
}
