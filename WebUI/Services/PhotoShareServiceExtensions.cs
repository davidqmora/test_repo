using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace WebUI.Services;

public static class PhotoShareServiceExtensions
{
    public static void AddLocalServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPhotoShareService, PhotoShareService>();
        services.AddHttpClient<ILegacyLoginService, LegacyLoginService>();

        _ = bool.TryParse(configuration["useMockApi"] ?? "false", out var useMockServices);
        
        if (useMockServices)
        {
            services.AddSingleton<LocalStore>();
            services.AddHttpClient<IAccountApiService, AccountApiServiceDev>().AddPolicyHandler(GetRetryPolicy());
            services.AddScoped<IUserAccountService, UserAccountServiceDev>();
        }
        else
        {
            services.AddHttpClient<IAccountApiService, AccountApiService>().AddPolicyHandler(GetRetryPolicy());
            services.AddScoped<IUserAccountService, UserAccountService>();
        }
    }

    private static Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}