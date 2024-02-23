using Polly;

namespace WebUI.Services;

public static class PhotoShareServiceExtensions
{
    public static void AddPhotoShareServices(
        this IServiceCollection services, IAsyncPolicy<HttpResponseMessage> retryPolicy)
    {
        services.AddHttpClient<IPhotoShareService, PhotoShareService>();
        services.AddHttpClient<ILegacyLoginService, LegacyLoginService>();
        services.AddHttpClient<ILocalAccountService, LocalAccountService>().AddPolicyHandler(retryPolicy);
    }
}