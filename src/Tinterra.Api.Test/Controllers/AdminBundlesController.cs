using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Application.Models;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/admin/bundles")]
[Authorize(Policy = "Security.Admin")]
public class AdminBundlesController : ControllerBase
{
    private readonly SecurityAdminService _service;

    public AdminBundlesController(SecurityAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetBundlesAsync(cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] PermissionBundleDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpsertBundleAsync(dto, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpGet("{bundle}/permissions")]
    public async Task<IActionResult> GetPermissions(string bundle, CancellationToken cancellationToken)
    {
        var result = await _service.GetBundlePermissionsAsync(bundle, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut("{bundle}/permissions")]
    public async Task<IActionResult> SetPermissions(string bundle, [FromBody] IReadOnlyCollection<string> permissions, CancellationToken cancellationToken)
    {
        var result = await _service.SetBundlePermissionsAsync(bundle, permissions, cancellationToken);
        return result.Succeeded ? Ok() : Problem(string.Join("; ", result.Errors));
    }
}
