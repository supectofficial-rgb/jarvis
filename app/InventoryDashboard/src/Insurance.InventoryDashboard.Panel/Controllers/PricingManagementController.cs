using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class PricingManagementController : Controller
{
    private readonly IApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    public PricingManagementController(IApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? item = null, CancellationToken cancellationToken = default)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await BuildPageModelAsync(token, item, cancellationToken);
        if (model.ActiveModule is null)
        {
            return Forbid();
        }

        model.StatusMessage = TempData["PricingStatus"] as string;
        model.ErrorMessage = TempData["PricingError"] as string ?? model.ErrorMessage;

        SetLayoutBags(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePriceType(UpsertPriceTypeRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            TempData["PricingError"] = "نام نوع قیمت الزامی است.";
            return RedirectToAction(nameof(Index), new { item = "price_types" });
        }

        var result = await _apiService.CreatePriceTypeAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "نوع قیمت ثبت شد."
            : result.ErrorMessage ?? "ثبت نوع قیمت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "price_types" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePriceType(Guid id, UpsertPriceTypeRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.UpdatePriceTypeAsync(id, request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "نوع قیمت به‌روزرسانی شد."
            : result.ErrorMessage ?? "به‌روزرسانی نوع قیمت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "price_types" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePriceChannel(UpsertPriceChannelRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            TempData["PricingError"] = "نام کانال قیمت الزامی است.";
            return RedirectToAction(nameof(Index), new { item = "price_channels" });
        }

        var result = await _apiService.CreatePriceChannelAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "کانال قیمت ثبت شد."
            : result.ErrorMessage ?? "ثبت کانال قیمت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "price_channels" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePriceChannel(Guid id, UpsertPriceChannelRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.UpdatePriceChannelAsync(id, request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "کانال قیمت به‌روزرسانی شد."
            : result.ErrorMessage ?? "به‌روزرسانی کانال قیمت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "price_channels" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVariantPrice(UpsertSellerVariantPriceRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var validationError = ValidateVariantPrice(request);
        if (validationError is not null)
        {
            TempData["PricingError"] = validationError;
            return RedirectToAction(nameof(Index), new { item = "variant_prices" });
        }

        var result = await _apiService.CreateSellerVariantPriceAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "قیمت واریانت ثبت شد."
            : result.ErrorMessage ?? "ثبت قیمت واریانت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "variant_prices" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVariantPrice(Guid id, UpsertSellerVariantPriceRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (request.Amount <= 0)
        {
            TempData["PricingError"] = "مبلغ قیمت باید بزرگ‌تر از صفر باشد.";
            return RedirectToAction(nameof(Index), new { item = "variant_prices" });
        }

        var result = await _apiService.UpdateSellerVariantPriceAsync(id, request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "قیمت واریانت به‌روزرسانی شد."
            : result.ErrorMessage ?? "به‌روزرسانی قیمت واریانت ناموفق بود.";

        return RedirectToAction(nameof(Index), new { item = "variant_prices" });
    }

    private async Task<PricingPageViewModel> BuildPageModelAsync(string token, string? item, CancellationToken cancellationToken)
    {
        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var model = new PricingPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Modules = modules,
            ActiveModule = modules.FirstOrDefault(x => string.Equals(x.ModuleId, "pricing_management", StringComparison.OrdinalIgnoreCase)),
        };

        var activeItem = string.IsNullOrWhiteSpace(item) ? "variant_prices" : item.Trim();
        model.ActiveItem = model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, activeItem, StringComparison.OrdinalIgnoreCase))
            ?? model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, "variant_prices", StringComparison.OrdinalIgnoreCase));

        var priceTypesTask = _apiService.SearchPriceTypesAsync(token, pageSize: 100);
        var priceTypeLookupTask = _apiService.GetPriceTypeLookupAsync(token);
        var priceChannelsTask = _apiService.SearchPriceChannelsAsync(token, pageSize: 100);
        var priceChannelLookupTask = _apiService.GetPriceChannelLookupAsync(token);
        var pricesTask = _apiService.SearchSellerVariantPricesAsync(token, pageSize: 100);
        var sellersTask = _apiService.GetSellerLookupAsync(token);
        var bucketsTask = _apiService.GetAvailableStockBucketsAsync(token, minQuantity: 0.0001m);

        await Task.WhenAll(priceTypesTask, priceTypeLookupTask, priceChannelsTask, priceChannelLookupTask, pricesTask, sellersTask, bucketsTask);

        Apply(model, priceTypesTask.Result, x => model.PriceTypes = x);
        Apply(model, priceTypeLookupTask.Result, x => model.PriceTypeLookup = x.Items);
        Apply(model, priceChannelsTask.Result, x => model.PriceChannels = x);
        Apply(model, priceChannelLookupTask.Result, x => model.PriceChannelLookup = x.Items);
        Apply(model, pricesTask.Result, x => model.VariantPrices = x);
        Apply(model, sellersTask.Result, x => model.Sellers = x.Items);
        Apply(model, bucketsTask.Result, x => model.AvailableBuckets = x.Items);

        return model;
    }

    private void SetLayoutBags(PricingPageViewModel model)
    {
        ViewBag.UserDisplayName = model.UserName;
        ViewBag.MenuModules = model.Modules;
        ViewBag.ActiveModuleId = model.ActiveModule?.ModuleId;
        ViewBag.ActiveItemId = model.ActiveItem?.ItemId;
    }

    private static void Apply<T>(PricingPageViewModel model, ApiResponse<T> response, Action<T> apply)
    {
        if (response is { IsSuccess: true, Data: not null })
        {
            apply(response.Data);
            return;
        }

        model.ErrorMessage ??= response.ErrorMessage ?? "بارگذاری اطلاعات قیمت ناموفق بود.";
    }

    private string? RequireToken()
    {
        var token = HttpContext.Session.GetString("Token");
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }

    private static string? ValidateVariantPrice(UpsertSellerVariantPriceRequest request)
    {
        if (request.SellerRef == Guid.Empty || request.VariantRef == Guid.Empty)
        {
            return "انتخاب بالانس/واریانت الزامی است.";
        }

        if (request.PriceTypeRef == Guid.Empty)
        {
            return "نوع قیمت الزامی است.";
        }

        if (request.PriceChannelRef == Guid.Empty)
        {
            return "کانال قیمت الزامی است.";
        }

        return request.Amount <= 0 ? "مبلغ قیمت باید بزرگ‌تر از صفر باشد." : null;
    }

    private IReadOnlyList<string> ResolveRolesFromSession(string token)
    {
        var rolesJson = HttpContext.Session.GetString("Roles");
        if (!string.IsNullOrWhiteSpace(rolesJson))
        {
            try
            {
                var roles = JsonSerializer.Deserialize<List<string>>(rolesJson) ?? new List<string>();
                if (roles.Count > 0)
                {
                    return roles
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch
            {
                // Recover from malformed session state by reading the JWT again.
            }
        }

        var extractedRoles = JwtRoleExtractor.ExtractRoles(token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(extractedRoles));
        return extractedRoles;
    }
}
