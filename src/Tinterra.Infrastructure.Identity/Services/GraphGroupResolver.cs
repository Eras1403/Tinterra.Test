using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
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
        var authProvider = new TokenAcquisitionAuthenticationProvider(tokenAcquisition);
        _graphServiceClient = new GraphServiceClient(authProvider);
    }

    public async Task<IReadOnlyCollection<string>> GetGroupObjectIdsAsync(string userObjectId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(userObjectId, out IReadOnlyCollection<string>? cached) && cached is not null)
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
        var response = await _graphServiceClient.Users[userObjectId].GetMemberGroups.PostAsGetMemberGroupsPostResponseAsync(new()
        {
            SecurityEnabledOnly = false
        }, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response?.Value?.Select(id => id.ToString()).ToList() ?? [];
    }

    private sealed class TokenAcquisitionAuthenticationProvider(ITokenAcquisition tokenAcquisition) : IAuthenticationProvider
    {
        private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;

        public async Task AuthenticateRequestAsync(
            RequestInformation request,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            var accessToken = await _tokenAcquisition
                .GetAccessTokenForUserAsync(GroupScopes)
                .ConfigureAwait(false);
            request.Headers.TryAdd("Authorization", $"Bearer {accessToken}");
        }
    }
}
