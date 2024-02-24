using WebUI.Model;

namespace WebUI.Services;

public interface IAccountApiService
{
    Task<HttpResponseMessage> GetAccountStatus(CancellationToken cancellationToken);
    Task<HttpResponseMessage> UpdateProfile(UserProfile profile, CancellationToken cancellationToken);
    // Task<AccountStatus?> GetMockAccountStatus(string? query, CancellationToken cancellationToken);
}