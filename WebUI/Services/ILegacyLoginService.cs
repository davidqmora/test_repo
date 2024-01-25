using Microsoft.AspNetCore.Mvc;

namespace WebUI.Services;

public interface ILegacyLoginService
{
    Task<string?> Login(string email);
}