using Microsoft.AspNetCore.Authorization;

namespace Tinterra.Api.Test.Authorization;

public enum ConfigurationAction
{
    View,
    Edit,
    Delete,
    Publish
}

public class ConfigurationAuthorizationRequirement : IAuthorizationRequirement
{
    public ConfigurationAuthorizationRequirement(ConfigurationAction action)
    {
        Action = action;
    }

    public ConfigurationAction Action { get; }
}
