using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Identity.Web;
using WebUI.Model;

namespace WebUI.Services;

public class LocalAccountService(ITokenAcquisition tokenAcquisition, IConfiguration configuration, HttpClient client) : ILocalAccountService
{  
    private readonly string apiReadScope = configuration["AccountsApi:ApiReaderScope"] ?? "";
    private readonly string apiBaseAddress = configuration["AccountsApi:BaseAddress"] ?? "";

    private const string Version = "v2";
    private const string AccountsRoute = "accounts";
    private const string ProfilesRoute = "profile";

    private readonly Dictionary<string, string> endpoints = new()
    {
        {"status", $"/{Version}/{AccountsRoute}/status"},
        {"status-dev", $"/{Version}/{AccountsRoute}/status-dev"},
        {"emails", $"/{Version}/{ProfilesRoute}/emails"},
        {"profile", $"/{Version}/{ProfilesRoute}"}
    };

    public async Task<AccountStatus?> GetAccountStatus(CancellationToken cancellationToken)
    {
        var response = await ApiGetOperation("status", cancellationToken);
        return await CreateAccountStatus(response);
    }

    public Task<HttpResponseMessage> GetProfileEmails(CancellationToken cancellationToken)
    {
        return ApiGetOperation("emails", cancellationToken);
    }

    public async Task<HttpResponseMessage> UpdateProfile(UserProfile profile, CancellationToken cancellationToken)
    {
         await InitializeClient();

         var serializedProfile = JsonSerializer.Serialize(profile);
         var updateRequest = new StringContent(serializedProfile, Encoding.UTF8, MediaTypeNames.Application.Json);

         return await client.PatchAsync($"{apiBaseAddress}{endpoints["profile"]}", updateRequest, cancellationToken);
    }

    public async Task<AccountStatus?> GetMockAccountStatus(string? query, CancellationToken cancellationToken)
    {
        var response = await ApiGetWithQueryOperation("status-dev", query, cancellationToken);
        return await CreateAccountStatus(response);
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


    private async Task<HttpResponseMessage> ApiGetOperation(string endpoint, CancellationToken cancellationToken)
    {
        await InitializeClient();
        return await client.GetAsync($"{apiBaseAddress}{endpoints[endpoint]}", cancellationToken);
    }

    private async Task<HttpResponseMessage> ApiGetWithQueryOperation(
        string endpoint,
        string? query,
        CancellationToken cancellationToken)
    {
        await InitializeClient();
        var urlTail = query == null ? "" : $"?{query}";
        return await client.GetAsync($"{apiBaseAddress}{endpoints[endpoint]}{urlTail}", cancellationToken);
    }

    private async Task InitializeClient()
    {
        var scopes = new[] { apiReadScope };
        var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("IdentityHubUI", "1.0"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Windows", "1234567890"));
    }
}