using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Application.Models;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/admin/tenants/{tenantId:guid}/group-mappings")]
[Authorize(Policy = "Security.Admin")]
public class AdminMappingsController : ControllerBase
{
    private readonly SecurityAdminService _service;

    public AdminMappingsController(SecurityAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _service.GetTenantGroupMappingsAsync(tenantId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(Guid tenantId, [FromBody] TenantGroupBundleMappingDto dto, CancellationToken cancellationToken)
    {
        var payload = dto with { TenantId = tenantId };
        var result = await _service.UpsertMappingAsync(payload, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid tenantId, [FromBody] TenantGroupBundleMappingDto dto, CancellationToken cancellationToken)
    {
        var payload = dto with { TenantId = tenantId };
        var result = await _service.DeleteMappingAsync(payload, cancellationToken);
        return result.Succeeded ? Ok() : Problem(string.Join("; ", result.Errors));
    }
}
