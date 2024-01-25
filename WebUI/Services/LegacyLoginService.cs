using System.Net.Http.Headers;
using System.Text;
using MyRadar.Accounts;
using MyRadar.Accounts.Accounts;
using MyRadar.Accounts.Installs;
using MyRadar.Accounts.Search;
using WebUI.Common;

namespace WebUI.Services;

public class LegacyLoginService(
    IAccountServicesFactory accountServicesFactory,
    IAccountSearchServicesFactory accountSearchServicesFactory,
    HttpClient client) : ILegacyLoginService
{
    private const string LoginUrl = "https://authc.acmeaom.com/v1/authenticate";
    
    // Just an arbitrary value for the moment.
    private const int TosaRequiredStatus = 399;
    
    public async Task<string?> Login(string email)
    {
        var installIds = await GetInstallIdFromEmail(email);
        
        if (installIds == null || installIds.Count == 0) return null;

        foreach (var installId in installIds)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{installId}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        
            var response = await client.GetAsync(LoginUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            if ((int)response.StatusCode == TosaRequiredStatus)
            {
                return NamedConstants.TOSARequired;
            }
        }

        return null;
    }
    
    private async Task<IReadOnlyCollection<InstallId>?> GetInstallIdFromEmail(string email)
    {
        var accountSearchService = accountSearchServicesFactory.Create();
        var userIdentity = UserIdentity.FromEmailIdentity(email);

        if (userIdentity == null) return null;
        
        var account = await accountSearchService.FindAccountAsync(userIdentity);
        if (account == null) return null;
        
        var accountService = accountServicesFactory.Create();
        var accountInstall = await accountService
            .GetRegisteredInstallsServiceAsync(account.AccountId);
        var installId = await accountInstall.GetInstallIdsAsync();
        
        return installId;
    }
    
}