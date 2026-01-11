using Microsoft.Identity.Client;

namespace Tinterra.Desktop.Test.Services;

public class MsalAuthService
{
    private readonly IPublicClientApplication _app;
    private readonly string[] _scopes;

    public MsalAuthService(string clientId, string redirectUri, string[] scopes)
    {
        _scopes = scopes;
        _app = PublicClientApplicationBuilder
            .Create(clientId)
            .WithAuthority("https://login.microsoftonline.com/organizations")
            .WithRedirectUri(redirectUri)
            .Build();
    }

    public async Task<AuthenticationResult> AcquireTokenAsync()
    {
        var accounts = await _app.GetAccountsAsync().ConfigureAwait(false);
        try
        {
            return await _app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync().ConfigureAwait(false);
        }
        catch (MsalUiRequiredException)
        {
            return await _app.AcquireTokenInteractive(_scopes)
                .ExecuteAsync().ConfigureAwait(false);
        }
    }
}
