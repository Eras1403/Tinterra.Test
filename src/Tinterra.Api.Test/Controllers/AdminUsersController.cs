using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinterra.Application.Models;
using Tinterra.Application.Services;

namespace Tinterra.Api.Test.Controllers;

[ApiController]
[Route("api/admin/users/{oid}/profile")]
[Authorize(Policy = "Security.Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly SecurityAdminService _service;

    public AdminUsersController(SecurityAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string oid, CancellationToken cancellationToken)
    {
        var result = await _service.GetUserProfileAsync(oid, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(string oid, [FromBody] UserProfileDto dto, CancellationToken cancellationToken)
    {
        var payload = dto with { ObjectId = oid };
        var result = await _service.UpsertUserProfileAsync(payload, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(string.Join("; ", result.Errors));
    }
}
