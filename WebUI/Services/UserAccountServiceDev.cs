using WebUI.Model;

namespace WebUI.Services;

public class UserAccountServiceDev : IUserAccountService
{
    private Entitlements entitlements = new Entitlements
    {
        PhotoShare = false,
        Investor = false,
        FoodTruck = false
    };
    
    
    public Task<AccountStatus?> GetStatus(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateProfile(UserProfile profile, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}