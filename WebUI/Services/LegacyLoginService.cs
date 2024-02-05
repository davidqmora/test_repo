using System.Net.Http.Headers;
using System.Text;
using MyRadar.Accounts;
using MyRadar.Accounts.Installs;
using WebUI.Common;

namespace WebUI.Services;

public class LegacyLoginService(
    IAccountServiceMaker accountServiceMaker,
    HttpClient client) : ILegacyLoginService
{
    private const string LoginUrl = "https://authc.acmeaom.com/v1/authenticate";
    
    // Just an arbitrary value for the moment.
    private const int TosaRequiredStatus = 399;
    
    public async Task<string?> Login(string email)
    {
        var installIds = await GetInstallIdsFromEmail(email);
        
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
    
    private async Task<IReadOnlyCollection<InstallId>?> GetInstallIdsFromEmail(string email)
    {
        var accountSearchService = accountServiceMaker.Create();
        var userIdentity = UserIdentity.FromEmailIdentity(email);

        if (userIdentity == null) return null;
        
        var account = await accountSearchService.AccountQueries.TryLookupByAsync(userIdentity);
        if (account == null) return null;
        
        var accountService = accountServiceMaker.Create();
        var installIds = await accountService
            .ById(account.AccountId)
            .Installs
            .GetInstallIdsAsync();
        
        return installIds;
    }
    
}