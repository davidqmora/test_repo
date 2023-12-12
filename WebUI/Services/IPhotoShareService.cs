namespace WebUI.Services;

public interface IPhotoShareService
{
    Task<HttpResponseMessage> Authorize(CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetPhotos(CancellationToken cancellationToken);
}