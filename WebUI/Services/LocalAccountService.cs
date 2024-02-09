using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Formatters;
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
        {"emails", $"/{Version}/{ProfilesRoute}/emails"},
        {"profile", $"/{Version}/{ProfilesRoute}"}
    };

    public Task<HttpResponseMessage> GetAccountStatus(CancellationToken cancellationToken)
    {
        return ApiGetOperation("status", cancellationToken);
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


    private async Task<HttpResponseMessage> ApiGetOperation(string endpoint, CancellationToken cancellationToken)
    {
        await InitializeClient();
        return await client.GetAsync($"{apiBaseAddress}{endpoints[endpoint]}", cancellationToken);
    }

    private async Task<HttpResponseMessage> PhotoApiOperation(string endpoint,
        string photoId,
        CancellationToken cancellationToken)
    {
        await InitializeClient();
        var path = string.Format(endpoints[endpoint], photoId);
        return await client.GetAsync($"{apiBaseAddress}{path}", cancellationToken);
    }

    private async Task InitializeClient()
    {
        var scopes = new[] { apiReadScope };
        var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LamarrUI", "1.0"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Windows", "1234567890"));
    }}