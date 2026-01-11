using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Security.Claims;
using Tinterra.Application.Interfaces;

namespace Tinterra.Infrastructure.Identity.Services;

public class GraphGroupResolver : IGroupResolver
{
    private static readonly string[] GroupScopes = ["https://graph.microsoft.com/.default"];
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;
    private readonly GraphServiceClient _graphServiceClient;

    public GraphGroupResolver(
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        ITokenAcquisition tokenAcquisition)
    {
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        var credential = new TokenAcquisitionTokenCredentialAdapter(tokenAcquisition);
        _graphServiceClient = new GraphServiceClient(credential, GroupScopes);
    }

    public async Task<IReadOnlyCollection<string>> GetGroupObjectIdsAsync(string userObjectId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(userObjectId, out IReadOnlyCollection<string> cached))
        {
            return cached;
        }

        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        var groupClaims = claimsPrincipal?.FindAll("groups").Select(c => c.Value).ToList() ?? [];

        if (groupClaims.Count == 0 && HasGroupOverage(claimsPrincipal))
        {
            groupClaims = await FetchGroupsFromGraphAsync(userObjectId, cancellationToken);
        }

        var groups = groupClaims.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        _cache.Set(userObjectId, groups, TimeSpan.FromMinutes(10));
        return groups;
    }

    private static bool HasGroupOverage(ClaimsPrincipal? principal)
    {
        var claimNames = principal?.FindFirst("_claim_names")?.Value;
        return !string.IsNullOrWhiteSpace(claimNames) && claimNames.Contains("groups", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<List<string>> FetchGroupsFromGraphAsync(string userObjectId, CancellationToken cancellationToken)
    {
        var response = await _graphServiceClient.Users[userObjectId].GetMemberGroups.PostAsync(new()
        {
            SecurityEnabledOnly = false
        }, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response?.Value?.Select(id => id.ToString()).ToList() ?? [];
    }
}
