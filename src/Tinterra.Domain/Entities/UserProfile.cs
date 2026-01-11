namespace Tinterra.Domain.Entities;

public class UserProfile
{
    public string ObjectId { get; set; } = string.Empty;
    public int ApprovalLevel { get; set; }
    public ICollection<UserAllowedRegion> AllowedRegions { get; set; } = new List<UserAllowedRegion>();
}
