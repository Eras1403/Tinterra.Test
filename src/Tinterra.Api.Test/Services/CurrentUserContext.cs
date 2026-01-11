using System.Security.Claims;
using Tinterra.Application.Interfaces;

namespace Tinterra.Api.Test.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string ObjectId => _httpContextAccessor.HttpContext?.User.FindFirstValue("oid") ?? string.Empty;

    public Guid TenantId
    {
        get
        {
            var tid = _httpContextAccessor.HttpContext?.User.FindFirstValue("tid");
            return Guid.TryParse(tid, out var parsed) ? parsed : Guid.Empty;
        }
    }

    public IReadOnlyCollection<string> Claims
        => _httpContextAccessor.HttpContext?.User.Claims.Select(c => $"{c.Type}:{c.Value}").ToList() ?? [];
}
