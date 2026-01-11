using System.Collections.ObjectModel;

namespace Tinterra.Desktop.Test.ViewModels;

public class MainViewModel
{
    public ObservableCollection<ConfigurationItemViewModel> ConfigurationItems { get; } =
        [
            new ConfigurationItemViewModel("ApiKey:Payments", "Prod / EU / Published"),
            new ConfigurationItemViewModel("Feature:NewOnboarding", "Test / CH / Draft")
        ];

    public ObservableCollection<AllowedTenantViewModel> Tenants { get; } =
        [
            new AllowedTenantViewModel("Contoso AG", true),
            new AllowedTenantViewModel("Fabrikam GmbH", false)
        ];

    public ObservableCollection<PermissionViewModel> Permissions { get; } =
        [
            new PermissionViewModel("Config.View"),
            new PermissionViewModel("Security.Admin")
        ];

    public ObservableCollection<PermissionBundleViewModel> Bundles { get; } =
        [
            new PermissionBundleViewModel("Config.Reader"),
            new PermissionBundleViewModel("Security.Admin")
        ];

    public ObservableCollection<TenantGroupBundleMappingViewModel> Mappings { get; } =
        [
            new TenantGroupBundleMappingViewModel("Tenant A -> Group X -> Config.Reader")
        ];

    public ObservableCollection<UserProfileViewModel> UserProfiles { get; } =
        [
            new UserProfileViewModel("User A (Approval 2, Regions: CH/EU)")
        ];
}

public record ConfigurationItemViewModel(string Key, string Summary);

public record AllowedTenantViewModel(string DisplayName, bool IsEnabled);

public record PermissionViewModel(string Name);

public record PermissionBundleViewModel(string Name);

public record TenantGroupBundleMappingViewModel(string Description);

public record UserProfileViewModel(string Description);
