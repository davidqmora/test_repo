using System.Net.Http.Headers;
using Microsoft.Identity.Web;

namespace WebUI.Services;

public class PhotoShareService(ITokenAcquisition acquisition, IConfiguration configuration, HttpClient client)
    : IPhotoShareService
{
    private readonly string apiScope = configuration["AuthorizePhotoShare:ApiScope"] ?? "";
    private readonly string apiBaseAddress = configuration["AuthorizePhotoShare:BaseAddress"] ?? "";


    public async Task<HttpResponseMessage> Authorize(CancellationToken cancellationToken)
    {
        await InitializeClient();
        var response = await client.GetAsync($"{apiBaseAddress}/v3/photoshare/authorize", cancellationToken);
        return response;
    }
    
    public async Task<HttpResponseMessage> GetMyPhotos(CancellationToken cancellationToken)
    {
        await InitializeClient();
        var response = await client.GetAsync($"{apiBaseAddress}/v3/photoshare/my-ids", cancellationToken);
        return response;
    }

    public async Task<HttpResponseMessage> GetMyMarkers(CancellationToken cancellationToken)
    {
        await InitializeClient();
        var response = await client.GetAsync($"{apiBaseAddress}/v3/photoshare/my-markers", cancellationToken);
        return response;
    }

    public async Task<HttpResponseMessage> GetMyVoteForPhoto(string photoId, CancellationToken cancellationToken)
    {
        await InitializeClient();
        var response = await client.GetAsync($"{apiBaseAddress}/v3/photoshare/{photoId}/my-vote", cancellationToken);
        return response;
    }

    public async Task<HttpResponseMessage> GetCommentsForPhoto(string photoId, CancellationToken cancellationToken)
    {
        await InitializeClient();
        return await client.GetAsync($"{apiBaseAddress}/v3/photoshare/{photoId}/comments", cancellationToken);
    }

    public async Task<HttpResponseMessage> GetDataForPhoto(string photoId, CancellationToken cancellationToken)
    {
        await InitializeClient();
        return await client.GetAsync($"{apiBaseAddress}/v3/photoshare/{photoId}/data", cancellationToken);
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