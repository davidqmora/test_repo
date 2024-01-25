namespace WebUI.Services;

public static class PhotoShareServiceExtensions
{
    public static void AddPhotoShareServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPhotoShareService, PhotoShareService>();
        services.AddHttpClient<ILegacyLoginService, LegacyLoginService>();
    }
}