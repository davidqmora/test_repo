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
        var response = await client.GetAsync($"{apiBaseAddress}/v1/photoshare/authorize", cancellationToken);
        return response;
    }
    
    public async Task<HttpResponseMessage> GetPhotos(CancellationToken cancellationToken)
    {
        await InitializeClient();
        var response = await client.GetAsync($"{apiBaseAddress}/v1/photoshare/photos", cancellationToken);
        return response;
    }

    private async Task InitializeClient()
    {
        var scopes = new[] { apiScope };
        var accessToken = await acquisition.GetAccessTokenForUserAsync(scopes);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}