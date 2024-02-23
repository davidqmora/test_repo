namespace WebUI.Model;

public class AccountStatus
{
    public UserStatus UserStatus { get; set; }
    public RequiredProperties? RequiredProperties { get; set; }
    public List<string>? AvailableEmails { get; set; }
}