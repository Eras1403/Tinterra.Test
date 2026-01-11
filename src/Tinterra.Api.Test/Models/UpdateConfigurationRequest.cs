using Tinterra.Domain.Enums;

namespace Tinterra.Api.Test.Models;

public record UpdateConfigurationRequest(string Value, ConfigurationClassification Classification, ConfigurationStatus Status);
