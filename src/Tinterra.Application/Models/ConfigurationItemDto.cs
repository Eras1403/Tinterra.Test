using Tinterra.Domain.Enums;

namespace Tinterra.Application.Models;

public record ConfigurationItemDto(
    Guid Id,
    string Key,
    string Value,
    ConfigurationEnvironment Environment,
    ConfigurationRegion Region,
    string OwnerObjectId,
    ConfigurationClassification Classification,
    ConfigurationStatus Status,
    int Version,
    DateTime UpdatedAtUtc);
