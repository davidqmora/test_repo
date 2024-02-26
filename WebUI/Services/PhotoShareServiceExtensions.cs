using Polly;

namespace WebUI.Services;

public static class PhotoShareServiceExtensions
{
    public static void AddLocalServices(
        this IServiceCollection services, IAsyncPolicy<HttpResponseMessage> retryPolicy, bool useMockServices = false)
    {
        services.AddHttpClient<IPhotoShareService, PhotoShareService>();
        services.AddHttpClient<ILegacyLoginService, LegacyLoginService>();

        if (useMockServices)
        {
            services.AddHttpClient<IAccountApiService, AccountApiServiceDev>().AddPolicyHandler(retryPolicy);
            services.AddScoped<IUserAccountService, UserAccountServiceDev>();
        }
        else
        {
            services.AddHttpClient<IAccountApiService, AccountApiService>().AddPolicyHandler(retryPolicy);
            services.AddScoped<IUserAccountService, UserAccountService>();
        }
    }
}