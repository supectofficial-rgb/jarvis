using System.Globalization;
using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class AuthController : Controller
{
    private readonly IAuthApiService _apiService;

    public AuthController(IAuthApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HasActiveToken())
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _apiService.LoginAsync(model.UserName, model.Password);
        if (!response.IsSuccess || response.Data is null)
        {
            ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "نام کاربری یا رمز عبور نامعتبر است.");
            return View(model);
        }

        var loginData = response.Data;
        var roles = ResolveRoles(loginData);

        HttpContext.Session.SetString("Token", loginData.Token);
        HttpContext.Session.SetString("RefreshToken", loginData.RefreshToken);
        HttpContext.Session.SetString("UserName", loginData.User?.UserName ?? model.UserName);
        HttpContext.Session.SetString("TokenExpiration", loginData.TokenExpiration.ToString("O"));
        if (loginData.ActiveOrganizationBusinessKey.HasValue)
        {
            HttpContext.Session.SetString("OrganizationBusinessKey", loginData.ActiveOrganizationBusinessKey.Value.ToString("D"));
        }
        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(roles));
        HttpContext.Session.SetString(
            "Permissions",
            JsonSerializer.Serialize(loginData.Permissions ?? new List<string>()));

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var accessToken = HttpContext.Session.GetString("Token") ?? string.Empty;
        var refreshToken = HttpContext.Session.GetString("RefreshToken") ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(accessToken) && !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _apiService.LogoutAsync(accessToken, refreshToken, "User logout");
        }

        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private bool HasActiveToken()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var expirationValue = HttpContext.Session.GetString("TokenExpiration");
        if (!DateTime.TryParse(expirationValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiration))
        {
            return true;
        }

        if (expiration > DateTime.UtcNow)
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("RefreshToken"));
    }

    private static List<string> ResolveRoles(LoginResponse loginData)
    {
        if (loginData.Roles is { Count: > 0 })
        {
            return loginData.Roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        return JwtRoleExtractor.ExtractRoles(loginData.Token)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
