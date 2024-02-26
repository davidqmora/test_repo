using WebUI.Model;

namespace WebUI.Services;

public interface IUserAccountService
{
    Task<AccountStatus?> GetStatus(CancellationToken cancellationToken);
    Task<bool> UpdateProfile(UserProfile profile, CancellationToken cancellationToken);
}