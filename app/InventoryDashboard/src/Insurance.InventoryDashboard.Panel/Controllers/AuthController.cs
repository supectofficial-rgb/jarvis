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
        HttpContext.Session.SetString("UserName", loginData.User?.UserName ?? model.UserName);
        HttpContext.Session.SetString("TokenExpiration", loginData.TokenExpiration.ToString("O"));
        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(roles));
        HttpContext.Session.SetString(
            "Permissions",
            JsonSerializer.Serialize(loginData.Permissions ?? new List<string>()));

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private bool HasActiveToken()
    {
        var token = HttpContext.Session.GetString("Token");
        return !string.IsNullOrWhiteSpace(token);
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

