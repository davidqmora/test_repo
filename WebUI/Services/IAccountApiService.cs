using WebUI.Model;

namespace WebUI.Services;

public interface IAccountApiService
{
    Task<HttpResponseMessage> GetAccountStatus(CancellationToken cancellationToken);
    Task<HttpResponseMessage> UpdateProfile(UserProfile profile, CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetEntitlements(CancellationToken cancellationToken);
}