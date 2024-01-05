namespace WebUI.Services;

public interface IPhotoShareService
{
    Task<HttpResponseMessage> Authorize(CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetMyPhotos(CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetMyMarkers(CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetMyVoteForPhoto(string photoId, CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetCommentsForPhoto(string photoId, CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetDataForPhoto(string photoId, CancellationToken none);
}