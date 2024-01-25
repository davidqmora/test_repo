using System.Net.Http.Headers;
using Microsoft.Identity.Web;

namespace WebUI.Services;

public class PhotoShareService(ITokenAcquisition acquisition, IConfiguration configuration, HttpClient client)
    : IPhotoShareService
{
    private readonly string apiScope = configuration["AuthorizePhotoShare:ApiScope"] ?? "";
    private readonly string apiBaseAddress = configuration["AuthorizePhotoShare:BaseAddress"] ?? "";

    private const string Version = "v2";
    private const string BaseRoute = "photos";
    
    private readonly Dictionary<string, string> endpoints = new Dictionary<string, string>
    {
        {"authorize", $"/{Version}/{BaseRoute}/authorize"},
        {"my-ids", $"/{Version}/{BaseRoute}/my-ids"},
        {"my-markers", $"/{Version}/{BaseRoute}/my-markers"},
        {"my-vote", $"/{Version}/{BaseRoute}/{{{0}}}/my-vote"},
        {"comments", $"/{Version}/{BaseRoute}/{{{0}}}/comments"},
        {"data", $"/{Version}/{BaseRoute}/{{{0}}}/data"}
    };


    public Task<HttpResponseMessage> Authorize(CancellationToken cancellationToken)
    {
        return GeneralApiOperation("authorize", cancellationToken);
    }

    public Task<HttpResponseMessage> GetMyPhotos(CancellationToken cancellationToken)
    {
        return GeneralApiOperation("my-ids", cancellationToken);
    }

    public  Task<HttpResponseMessage> GetMyMarkers(CancellationToken cancellationToken)
    {
        return GeneralApiOperation("my-markers", cancellationToken);
    }

    public Task<HttpResponseMessage> GetMyVoteForPhoto(string photoId, CancellationToken cancellationToken)
    {
        return PhotoApiOperation("my-vote", photoId, cancellationToken);
    }
    
    public  Task<HttpResponseMessage> GetCommentsForPhoto(string photoId, CancellationToken cancellationToken)
    {
        return PhotoApiOperation("comments", photoId, cancellationToken);
    }

    public Task<HttpResponseMessage> GetDataForPhoto(string photoId, CancellationToken cancellationToken)
    {
        return PhotoApiOperation("data", photoId, cancellationToken);
    }

    private async Task<HttpResponseMessage> GeneralApiOperation(string endpoint, CancellationToken cancellationToken)
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
        var scopes = new[] { apiScope };
        var accessToken = await acquisition.GetAccessTokenForUserAsync(scopes);
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LamarrUI", "1.0"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Windows", "1234567890"));
    }
}