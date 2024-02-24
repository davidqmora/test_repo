using WebUI.Model;

namespace WebUI.Services;

public interface IUserAccountService
{
    Task<AccountStatus?> GetStatus(CancellationToken cancellationToken);
}