using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Application.Models;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/admin/tenants")]
[Authorize(Policy = "Security.Admin")]
public class AdminTenantsController : ControllerBase
{
    private readonly SecurityAdminService _service;

    public AdminTenantsController(SecurityAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetTenantsAsync(cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] AllowedTenantDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpsertTenantAsync(dto, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }
}
