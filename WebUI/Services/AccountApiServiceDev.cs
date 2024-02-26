﻿using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Graph.Models.TermStore;
using Microsoft.Identity.Web;
using WebUI.Model;

namespace WebUI.Services;

public class AccountApiServiceDev(ITokenAcquisition tokenAcquisition, IConfiguration configuration, HttpClient client) : IAccountApiService
{
    private readonly string apiReadScope = configuration["AccountsApi:ApiReaderScope"] ?? "";
    private readonly string apiBaseAddress = configuration["AccountsApi:BaseAddress"] ?? "";

    private const string Version = "v2";
    private const string AccountsRoute = "accounts";
    private const string ProfilesRoute = "profile";

    private readonly Dictionary<string, string> endpoints = new()
    {
        {"status", $"/{Version}/{AccountsRoute}/status-dev"},
        {"emails", $"/{Version}/{ProfilesRoute}/emails"},
        {"profile", $"/{Version}/{ProfilesRoute}"}
    };

    private Entitlements? entitlementsSettings;
    

    public Task<HttpResponseMessage> GetAccountStatus(CancellationToken cancellationToken)
    {
        var query = string.Empty;
        if (entitlementsSettings is not null)
        {
            query = GetQueryFromEntitlements();
        }
        return ApiGetWithQueryOperation("status", query, cancellationToken);
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

    public void SetEntitlements(Entitlements entitlements)
    {
        entitlementsSettings = entitlements;
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
    
    private string GetQueryFromEntitlements()
    {
        var jsonEntitlements = JsonSerializer.SerializeToElement(entitlementsSettings);
        return string.Join("&", jsonEntitlements.EnumerateObject().Select(entitlement => $"{entitlement.Name}={entitlement.Value.GetBoolean()}"));
    }
}