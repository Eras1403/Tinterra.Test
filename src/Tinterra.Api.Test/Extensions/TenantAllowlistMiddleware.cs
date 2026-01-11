using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tinterra.Application.Interfaces;

namespace Tinterra.Api.Test.Extensions;

public class TenantAllowlistMiddleware
{
    private readonly RequestDelegate _next;

    public TenantAllowlistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAllowedTenantRepository repository, IMemoryCache cache)
    {
        var tenantIdClaim = context.User.FindFirst("tid")?.Value;
        if (string.IsNullOrWhiteSpace(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            await _next(context);
            return;
        }

        var cacheKey = $"tenant:{tenantId}";
        if (!cache.TryGetValue(cacheKey, out bool isAllowed))
        {
            var tenant = await repository.GetByIdAsync(tenantId, context.RequestAborted);
            isAllowed = tenant?.IsEnabled ?? false;
            cache.Set(cacheKey, isAllowed, TimeSpan.FromMinutes(5));
        }

        if (!isAllowed)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Tenant is not allowed",
                Detail = "The tenant is disabled or not present in the allow list."
            };

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        await _next(context);
    }
}
