using Tinterra.Application.Interfaces;
using Tinterra.Application.Models;
using Tinterra.Domain.Entities;
using Tinterra.Domain.Enums;

namespace Tinterra.Application.Services;

public class ConfigurationService
{
    private readonly IConfigurationRepository _repository;
    private readonly ICurrentUserContext _currentUser;

    public ConfigurationService(IConfigurationRepository repository, ICurrentUserContext currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyCollection<ConfigurationItemDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<ConfigurationItemDto>>.Success(items.Select(ToDto).ToList());
    }

    public async Task<Result<ConfigurationItemDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return Result<ConfigurationItemDto>.Failure("Configuration item not found.");
        }

        return Result<ConfigurationItemDto>.Success(ToDto(item));
    }

    public async Task<Result<ConfigurationItemDto>> UpdateAsync(Guid id, string value, ConfigurationClassification classification, ConfigurationStatus status, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return Result<ConfigurationItemDto>.Failure("Configuration item not found.");
        }

        item.Value = value;
        item.Classification = classification;
        item.Status = status;
        item.Version += 1;
        item.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(item, cancellationToken);
        return Result<ConfigurationItemDto>.Success(ToDto(item));
    }

    public async Task<Result<ConfigurationItemDto>> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return Result<ConfigurationItemDto>.Failure("Configuration item not found.");
        }

        item.Status = ConfigurationStatus.Published;
        item.Version += 1;
        item.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(item, cancellationToken);
        return Result<ConfigurationItemDto>.Success(ToDto(item));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return Result<bool>.Failure("Configuration item not found.");
        }

        await _repository.DeleteAsync(item, cancellationToken);
        return Result<bool>.Success(true);
    }

    private static ConfigurationItemDto ToDto(ConfigurationItem item)
    {
        return new ConfigurationItemDto(
            item.Id,
            item.Key,
            item.Value,
            item.Environment,
            item.Region,
            item.OwnerObjectId,
            item.Classification,
            item.Status,
            item.Version,
            item.UpdatedAtUtc);
    }
}
