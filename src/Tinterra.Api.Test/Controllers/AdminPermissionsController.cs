using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Application.Models;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/admin/permissions")]
[Authorize(Policy = "Security.Admin")]
public class AdminPermissionsController : ControllerBase
{
    private readonly SecurityAdminService _service;

    public AdminPermissionsController(SecurityAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetPermissionsAsync(cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] PermissionDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpsertPermissionAsync(dto, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }
}
