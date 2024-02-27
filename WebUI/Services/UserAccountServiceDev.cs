using System.Text.Json;
using WebUI.Model;

namespace WebUI.Services;

public class UserAccountServiceDev(IAccountApiService accountApiService) : IUserAccountService
{
    private AccountStatus? accountStatus;
    private Entitlements? entitlements;

    public async Task<AccountStatus?> GetStatus(CancellationToken cancellationToken)
    {
        if (accountStatus is null)
        {
            var response = await accountApiService.GetAccountStatus(cancellationToken);
            accountStatus = await CreateAccountStatus(response);
        }

        return accountStatus;
    }

    public Task<bool> UpdateProfile(UserProfile profile, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Entitlements?> GetEntitlements(CancellationToken cancellationToken)
    {
        if (entitlements is null)
        {
            var response = await accountApiService.GetEntitlements(cancellationToken);
            await ParseEntitlements(response);
        }

        return entitlements;
    }   

    private static async Task<AccountStatus?> CreateAccountStatus(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return null;
        
        var jsonContent = await response.Content.ReadAsStringAsync();

        var accountStatus = new AccountStatus();

        using var jsonDoc = JsonDocument.Parse(jsonContent);
        accountStatus.UserStatus = jsonDoc.RootElement.GetProperty("status").Deserialize<UserStatus>();

        if (jsonDoc
            .RootElement
            .TryGetProperty("required_missing_properties", out var requiredPropertiesElement))
        {
            accountStatus.RequiredProperties = requiredPropertiesElement.Deserialize<RequiredProperties>();
        }

        if (jsonDoc
            .RootElement
            .TryGetProperty("available_emails", out var availableEmailsElement))
        {
            accountStatus.AvailableEmails = availableEmailsElement.Deserialize<List<string>>();
        }

        return accountStatus;
    }
    
    private async Task ParseEntitlements(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return;

        var entitlementsResponse = await response.Content.ReadAsStringAsync();

        if (entitlements is null)
        {
            entitlements = new Entitlements();
        }
        
        entitlements.Investor = entitlementsResponse.Contains("investor");
        entitlements.FoodTruck = entitlementsResponse.Contains("food_truck");
        entitlements.PhotoShare = entitlementsResponse.Contains("photo_share");
    }
}