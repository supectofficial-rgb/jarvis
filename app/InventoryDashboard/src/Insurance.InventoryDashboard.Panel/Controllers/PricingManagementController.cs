using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class PricingManagementController : Controller
{
    private const int VariantSearchPageSize = 25;

    private readonly IApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    public PricingManagementController(IApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? item = null, string? variantSearchTerm = null, CancellationToken cancellationToken = default)
        => await ShowPricingPageAsync(string.IsNullOrWhiteSpace(item) ? "variant_prices" : item, variantSearchTerm, cancellationToken);

    [HttpGet]
    public async Task<IActionResult> VariantPrices(string? variantSearchTerm = null, CancellationToken cancellationToken = default)
        => await ShowPricingPageAsync("variant_prices", variantSearchTerm, cancellationToken);

    [HttpGet]
    public async Task<IActionResult> PriceTypes(CancellationToken cancellationToken = default)
        => await ShowPricingPageAsync("price_types", null, cancellationToken);

    [HttpGet]
    public async Task<IActionResult> PriceChannels(CancellationToken cancellationToken = default)
        => await ShowPricingPageAsync("price_channels", null, cancellationToken);

    private async Task<IActionResult> ShowPricingPageAsync(string item, string? variantSearchTerm, CancellationToken cancellationToken)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await BuildPageModelAsync(token, item, variantSearchTerm, cancellationToken);
        if (model.ActiveModule is null)
        {
            return Forbid();
        }

        model.StatusMessage = TempData["PricingStatus"] as string;
        model.ErrorMessage = TempData["PricingError"] as string ?? model.ErrorMessage;

        SetLayoutBags(model);
        return View("~/Views/PricingManagement/Index.cshtml", model);
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
            return RedirectToAction(nameof(PriceTypes));
        }

        var result = await _apiService.CreatePriceTypeAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "نوع قیمت ثبت شد."
            : result.ErrorMessage ?? "ثبت نوع قیمت ناموفق بود.";

        return RedirectToAction(nameof(PriceTypes));
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

        return RedirectToAction(nameof(PriceTypes));
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
            return RedirectToAction(nameof(PriceChannels));
        }

        var result = await _apiService.CreatePriceChannelAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "کانال قیمت ثبت شد."
            : result.ErrorMessage ?? "ثبت کانال قیمت ناموفق بود.";

        return RedirectToAction(nameof(PriceChannels));
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

        return RedirectToAction(nameof(PriceChannels));
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
            return RedirectToAction(nameof(VariantPrices));
        }

        var result = await _apiService.CreateSellerVariantPriceAsync(request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "قیمت واریانت ثبت شد."
            : result.ErrorMessage ?? "ثبت قیمت واریانت ناموفق بود.";

        return RedirectToAction(nameof(VariantPrices));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyVariantPricing(BulkVariantPricingRequest request)
    {
        var token = RequireToken();
        if (token is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var ownerSeller = await ResolveOwnerSellerAsync(token);
        if (ownerSeller is null)
        {
            TempData["PricingError"] = "برای قیمت‌گذاری باید Seller Owner فعال در سیستم ثبت شده باشد.";
            return RedirectToAction(nameof(VariantPrices));
        }

        var variantRefs = request.SelectedVariantRefs
            .Select(ParseGuidOrDefault)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (variantRefs.Count == 0)
        {
            TempData["PricingError"] = "حداقل یک واریانت را برای قیمت‌گذاری انتخاب کنید.";
            return RedirectToAction(nameof(VariantPrices));
        }

        var entries = BuildPriceEntries(request);
        if (entries.Count == 0)
        {
            TempData["PricingError"] = "حداقل یک مبلغ معتبر برای یکی از ترکیب‌های نوع قیمت و کانال وارد کنید.";
            return RedirectToAction(nameof(VariantPrices));
        }

        var sellerPricesByVariant = new Dictionary<Guid, List<SellerVariantPriceModel>>();
        foreach (var variantRef in variantRefs)
        {
            var searchResult = await _apiService.SearchSellerVariantPricesAsync(
                token,
                sellerRef: ownerSeller.SellerBusinessKey,
                variantRef: variantRef,
                pageSize: 200);

            if (!searchResult.IsSuccess)
            {
                TempData["PricingError"] = searchResult.ErrorMessage ?? "بارگذاری قیمت‌های فعلی واریانت ناموفق بود.";
                return RedirectToAction(nameof(VariantPrices));
            }

            sellerPricesByVariant[variantRef] = searchResult.Data?.Items ?? new List<SellerVariantPriceModel>();
        }

        var processedCount = 0;
        foreach (var variantRef in variantRefs)
        {
            var existingPrices = sellerPricesByVariant[variantRef];
            foreach (var entry in entries)
            {
                var payload = new UpsertSellerVariantPriceRequest
                {
                    SellerRef = ownerSeller.SellerBusinessKey,
                    VariantRef = variantRef,
                    PriceTypeRef = entry.PriceTypeRef,
                    PriceChannelRef = entry.PriceChannelRef,
                    Amount = entry.Amount!.Value,
                    Currency = string.IsNullOrWhiteSpace(request.Currency) ? "IRR" : request.Currency.Trim().ToUpperInvariant(),
                    MinQty = request.MinQty <= 0 ? 1 : request.MinQty,
                    Priority = request.Priority,
                    IsActive = true
                };

                var existing = existingPrices.FirstOrDefault(x =>
                    x.PriceTypeRef == entry.PriceTypeRef &&
                    x.PriceChannelRef == entry.PriceChannelRef);

                var result = existing is null
                    ? await _apiService.CreateSellerVariantPriceAsync(payload, token)
                    : await _apiService.UpdateSellerVariantPriceAsync(existing.SellerVariantPriceBusinessKey, payload, token);

                if (!result.IsSuccess)
                {
                    TempData["PricingError"] = result.ErrorMessage ?? "ثبت قیمت واریانت ناموفق بود.";
                    return RedirectToAction(nameof(VariantPrices));
                }

                processedCount++;
            }
        }

        TempData["PricingStatus"] = $"{processedCount} قیمت برای Seller Owner ثبت یا به‌روزرسانی شد.";
        return RedirectToAction(nameof(VariantPrices));
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
            return RedirectToAction(nameof(VariantPrices));
        }

        var result = await _apiService.UpdateSellerVariantPriceAsync(id, request, token);
        TempData[result.IsSuccess ? "PricingStatus" : "PricingError"] = result.IsSuccess
            ? "قیمت واریانت به‌روزرسانی شد."
            : result.ErrorMessage ?? "به‌روزرسانی قیمت واریانت ناموفق بود.";

        return RedirectToAction(nameof(VariantPrices));
    }

    private async Task<PricingPageViewModel> BuildPageModelAsync(string token, string? item, string? variantSearchTerm, CancellationToken cancellationToken)
    {
        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var model = new PricingPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Modules = modules,
            ActiveModule = modules.FirstOrDefault(x => string.Equals(x.ModuleId, "pricing_management", StringComparison.OrdinalIgnoreCase)),
            VariantSearchTerm = variantSearchTerm
        };

        var activeItem = string.IsNullOrWhiteSpace(item) ? "variant_prices" : item.Trim();
        model.ActiveItem = model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, activeItem, StringComparison.OrdinalIgnoreCase))
            ?? model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, "variant_prices", StringComparison.OrdinalIgnoreCase));

        var priceTypesTask = _apiService.SearchPriceTypesAsync(token, pageSize: 100);
        var priceTypeLookupTask = _apiService.GetPriceTypeLookupAsync(token);
        var priceChannelsTask = _apiService.SearchPriceChannelsAsync(token, pageSize: 100);
        var priceChannelLookupTask = _apiService.GetPriceChannelLookupAsync(token);
        var sellersTask = _apiService.GetSellerLookupAsync(token);
        var ownerSellerTask = _apiService.SearchSellersAsync(token, isSystemOwner: true, isActive: true, pageSize: 10);
        var variantsTask = _apiService.SearchProductVariantsAsync(token, searchTerm: variantSearchTerm, isActive: true, pageSize: VariantSearchPageSize);

        await Task.WhenAll(priceTypesTask, priceTypeLookupTask, priceChannelsTask, priceChannelLookupTask, sellersTask, ownerSellerTask, variantsTask);

        Apply(model, priceTypesTask.Result, x => model.PriceTypes = x);
        Apply(model, priceTypeLookupTask.Result, x => model.PriceTypeLookup = x.Items ?? new List<PriceTypeLookupModel>());
        Apply(model, priceChannelsTask.Result, x => model.PriceChannels = x);
        Apply(model, priceChannelLookupTask.Result, x => model.PriceChannelLookup = x.Items ?? new List<PriceChannelLookupModel>());
        Apply(model, sellersTask.Result, x => model.Sellers = x.Items ?? new List<SellerLookupModel>());
        Apply(model, variantsTask.Result, x => model.VariantSearchResult = x);

        var ownerSellers = ownerSellerTask.Result.Data?.Items?
            .Where(x => x.IsSystemOwner && x.IsActive)
            .OrderBy(x => x.Code)
            .ToList() ?? new List<SellerSearchItemModel>();

        if (ownerSellers.Count == 1)
        {
            model.OwnerSeller = ownerSellers[0];
        }

        if (ownerSellers.Count == 0)
        {
            model.ErrorMessage ??= "Seller Owner فعال در سیستم پیدا نشد. تا زمان ثبت Seller Owner قیمت‌گذاری این بخش غیرفعال است.";
        }
        else if (ownerSellers.Count > 1)
        {
            model.ErrorMessage ??= "بیش از یک Seller Owner فعال پیدا شد. تا زمان تعیین یک Owner یکتا قیمت‌گذاری این بخش غیرفعال است.";
        }
        else
        {
            var ownerSeller = model.OwnerSeller!;
            var pricesResult = await _apiService.SearchSellerVariantPricesAsync(token, sellerRef: ownerSeller.SellerBusinessKey, pageSize: 200);
            Apply(model, pricesResult, x => model.VariantPrices = x);
        }

        model.BulkPricingForm.Prices = BuildPriceMatrix(
            model.PriceTypeLookup ?? new List<PriceTypeLookupModel>(),
            model.PriceChannelLookup ?? new List<PriceChannelLookupModel>());

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
            return "انتخاب واریانت الزامی است.";
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

    private async Task<SellerSearchItemModel?> ResolveOwnerSellerAsync(string token)
    {
        var result = await _apiService.SearchSellersAsync(token, isSystemOwner: true, isActive: true, pageSize: 10);
        if (!result.IsSuccess)
        {
            return null;
        }

        var ownerSellers = result.Data?.Items?
            .Where(x => x.IsSystemOwner && x.IsActive)
            .OrderBy(x => x.Code)
            .ToList() ?? new List<SellerSearchItemModel>();

        return ownerSellers.Count == 1 ? ownerSellers[0] : null;
    }

    private static List<VariantPriceMatrixInputModel> BuildPriceMatrix(
        IEnumerable<PriceTypeLookupModel> priceTypes,
        IEnumerable<PriceChannelLookupModel> priceChannels)
    {
        var uniquePriceTypes = priceTypes
            .GroupBy(x => x.PriceTypeBusinessKey)
            .Select(x => x.First())
            .OrderBy(x => x.Name)
            .ToList();
        var uniquePriceChannels = priceChannels
            .GroupBy(x => x.PriceChannelBusinessKey)
            .Select(x => x.First())
            .OrderBy(x => x.Name)
            .ToList();

        return (from priceType in uniquePriceTypes
                from priceChannel in uniquePriceChannels
                select new VariantPriceMatrixInputModel
                {
                    PriceTypeRef = priceType.PriceTypeBusinessKey,
                    PriceChannelRef = priceChannel.PriceChannelBusinessKey
                }).ToList();
    }

    private static List<VariantPriceMatrixInputModel> BuildPriceEntries(BulkVariantPricingRequest request)
    {
        var fallbackAmount = request.ApplyToAllAmount;
        return request.Prices
            .Select(x => new VariantPriceMatrixInputModel
            {
                PriceTypeRef = x.PriceTypeRef,
                PriceChannelRef = x.PriceChannelRef,
                Amount = x.Amount ?? fallbackAmount
            })
            .Where(x => x.Amount.HasValue && x.Amount.Value > 0)
            .ToList();
    }

    private static Guid ParseGuidOrDefault(string? value)
        => Guid.TryParse(value, out var parsed) ? parsed : Guid.Empty;

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
