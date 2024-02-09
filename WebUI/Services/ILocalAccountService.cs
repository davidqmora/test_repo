using WebUI.Model;

namespace WebUI.Services;

public interface ILocalAccountService
{
    Task<HttpResponseMessage> GetAccountStatus(CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetProfileEmails(CancellationToken cancellationToken);
    Task<HttpResponseMessage> UpdateProfile(UserProfile profile, CancellationToken cancellationToken);
}