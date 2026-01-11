using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Api.Test.Authorization;
using Tinterra.Api.Test.Models;
using Tinterra.Application.Interfaces;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/configurations")]
[Authorize]
public class ConfigurationsController : ControllerBase
{
    private readonly ConfigurationService _service;
    private readonly IConfigurationRepository _repository;
    private readonly IAuthorizationService _authorizationService;

    public ConfigurationsController(
        ConfigurationService service,
        IConfigurationRepository repository,
        IAuthorizationService authorizationService)
    {
        _service = service;
        _repository = repository;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        if (!result.Succeeded)
        {
            return Problem(string.Join("; ", result.Errors));
        }

        var filtered = new List<object>();
        foreach (var item in result.Value ?? [])
        {
            var entity = await _repository.GetByIdAsync(item.Id, cancellationToken);
            if (entity is null)
            {
                continue;
            }

            var authorized = await _authorizationService.AuthorizeAsync(User, entity, new ConfigurationAuthorizationRequirement(ConfigurationAction.View));
            if (authorized.Succeeded)
            {
                filtered.Add(item);
            }
        }

        return Ok(filtered);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var authorized = await _authorizationService.AuthorizeAsync(User, entity, new ConfigurationAuthorizationRequirement(ConfigurationAction.View));
        if (!authorized.Succeeded)
        {
            return Forbid();
        }

        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConfigurationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var authorized = await _authorizationService.AuthorizeAsync(User, entity, new ConfigurationAuthorizationRequirement(ConfigurationAction.Edit));
        if (!authorized.Succeeded)
        {
            return Forbid();
        }

        var result = await _service.UpdateAsync(id, request.Value, request.Classification, request.Status, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var authorized = await _authorizationService.AuthorizeAsync(User, entity, new ConfigurationAuthorizationRequirement(ConfigurationAction.Publish));
        if (!authorized.Succeeded)
        {
            return Forbid();
        }

        var result = await _service.PublishAsync(id, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var authorized = await _authorizationService.AuthorizeAsync(User, entity, new ConfigurationAuthorizationRequirement(ConfigurationAction.Delete));
        if (!authorized.Succeeded)
        {
            return Forbid();
        }

        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Succeeded ? Ok() : Problem(string.Join("; ", result.Errors));
    }
}
