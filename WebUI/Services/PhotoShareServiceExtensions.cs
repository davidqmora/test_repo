namespace WebUI.Services;

public static class PhotoShareServiceExtensions
{
    public static void AddPhotoShareService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPhotoShareService, PhotoShareService>();
    }
}