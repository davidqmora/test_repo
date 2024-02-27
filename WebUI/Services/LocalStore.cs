using WebUI.Model;

namespace WebUI.Services;

public class LocalStore
{
    public string? AccountStatusQuery { get; set; }
    public string? EntitlementsQuery { get; set; }

    public Entitlements? EntitlementsSpec { get; set; }
}