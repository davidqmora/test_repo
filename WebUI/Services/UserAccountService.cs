﻿using System.Text.Json;
using WebUI.Model;

namespace WebUI.Services;

public class UserAccountService(IAccountApiService accountApiService) : IUserAccountService
{
    private AccountStatus? accountStatus;
    
    public async Task<AccountStatus?> GetStatus(CancellationToken cancellationToken)
    {
        if (accountStatus is null)
        {
            var response = await accountApiService.GetAccountStatus(cancellationToken);
            accountStatus =  await CreateAccountStatus(response);
        }

        return accountStatus;
    }

    public async Task<bool> UpdateProfile(UserProfile profile, CancellationToken cancellationToken)
    {
        var response = await accountApiService.UpdateProfile(profile, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Unable to submit PhotoShare profile update: {response.StatusCode} - {response.ReasonPhrase}");
            return false;
        }
        
        accountStatus!.UserStatus = UserStatus.Enabled;
        accountStatus.RequiredProperties = null;
        return true;
    }
    
    private static async Task<AccountStatus?> CreateAccountStatus(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

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
}