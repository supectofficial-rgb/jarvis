using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class InventoryDocumentsController : Controller
{
    private static readonly int[] PageSizeOptions = [10, 25, 50];

    private readonly IApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    public InventoryDocumentsController(IApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Documents));

    [HttpGet]
    public async Task<IActionResult> Documents(
        string? variantSearchTerm,
        string? documentNo,
        string? documentType,
        string? status,
        string? warehouseId,
        int documentPage = 1,
        int documentPageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var activeModule = modules.FirstOrDefault(x => string.Equals(x.ModuleId, "inventory_documents", StringComparison.OrdinalIgnoreCase));
        var activeItem = activeModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, "documents", StringComparison.OrdinalIgnoreCase));

        documentPageSize = PageSizeOptions.Contains(documentPageSize) ? documentPageSize : 10;

        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: false);
        var locationsTask = _apiService.SearchLocationsAsync(token, pageSize: 200);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: false);
        var qualityTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var variantsTask = _apiService.SearchProductVariantsAsync(token, searchTerm: variantSearchTerm, isActive: true, pageSize: 25);
        var documentsTask = _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo,
            documentType,
            status,
            warehouseId,
            Math.Max(documentPage, 1),
            documentPageSize);

        await Task.WhenAll(warehousesTask, locationsTask, sellersTask, qualityTask, variantsTask, documentsTask);

        var warehouses = warehousesTask.Result.Data ?? new List<WarehouseLookupItemModel>();
        var sellers = sellersTask.Result.Data?.Items ?? new List<SellerLookupModel>();

        var model = new InventoryDocumentManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = activeModule,
            ActiveItem = activeItem,
            Warehouses = warehouses,
            Locations = locationsTask.Result.Data?.Items ?? new List<LocationListItemModel>(),
            Sellers = sellers,
            QualityStatuses = qualityTask.Result.Data?.Items ?? new List<QualityStatusLookupModel>(),
            VariantSearchResult = variantsTask.Result.Data ?? new ProductVariantSearchResultModel(),
            Documents = documentsTask.Result.Data ?? new InventoryDocumentSearchResultModel(),
            VariantSearchTerm = variantSearchTerm,
            DocumentNoFilter = documentNo,
            DocumentTypeFilter = documentType,
            StatusFilter = status,
            WarehouseFilterId = warehouseId,
            DocumentPage = documentsTask.Result.Data?.Page ?? Math.Max(documentPage, 1),
            DocumentPageSize = documentsTask.Result.Data?.PageSize ?? documentPageSize,
            PageSizeOptions = PageSizeOptions,
            ErrorMessage = new[]
            {
                warehousesTask.Result.ErrorMessage,
                locationsTask.Result.ErrorMessage,
                sellersTask.Result.ErrorMessage,
                qualityTask.Result.ErrorMessage,
                variantsTask.Result.ErrorMessage,
                documentsTask.Result.ErrorMessage
            }.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
            DocumentForm = new InventoryDocumentForm
            {
                WarehouseId = warehouseId ?? warehouses.FirstOrDefault()?.WarehouseBusinessKey ?? string.Empty,
                SellerId = sellers.FirstOrDefault()?.SellerBusinessKey.ToString("D") ?? string.Empty,
                OccurredAt = DateTime.Today
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDocument([Bind(Prefix = "DocumentForm")] InventoryDocumentForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var lines = ParseLines(form.LinesJson);
        if (lines.Count == 0)
        {
            TempData["CatalogError"] = "برای سند حداقل یک آیتم انتخاب کنید.";
            return RedirectToAction(nameof(Documents));
        }

        var validationError = ValidateLines(form.DocumentType, form.ReasonCode, lines);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            TempData["CatalogError"] = validationError;
            return RedirectToAction(nameof(Documents));
        }

        var createResult = await _apiService.CreateInventoryDocumentAsync(form, lines, token);
        if (!createResult.IsSuccess)
        {
            TempData["CatalogError"] = createResult.ErrorMessage ?? "ایجاد سند با خطا مواجه شد.";
            return RedirectToAction(nameof(Documents));
        }

        if (form.PostAfterCreate)
        {
            var postResult = await _apiService.PostInventoryDocumentAsync(createResult.Data, token);
            TempData[postResult.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
                postResult.IsSuccess ? "سند ایجاد و نهایی‌سازی شد." : postResult.ErrorMessage ?? "سند ایجاد شد اما نهایی‌سازی با خطا مواجه شد.";
        }
        else
        {
            TempData["CatalogSuccess"] = "سند به صورت پیش‌نویس ذخیره شد.";
        }

        return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseId });
    }

    [HttpGet]
    public async Task<IActionResult> DocumentItems(Guid documentId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        var result = await _apiService.GetInventoryDocumentLinesAsync(documentId, token);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data ?? new List<InventoryDocumentLineItemModel>());
    }

    private static List<InventoryDocumentLineForm> ParseLines(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<InventoryDocumentLineForm>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<InventoryDocumentLineForm>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?
                .Where(x => !string.IsNullOrWhiteSpace(x.VariantId) && x.Quantity > 0)
                .ToList() ?? new List<InventoryDocumentLineForm>();
        }
        catch
        {
            return new List<InventoryDocumentLineForm>();
        }
    }

    private static string? ValidateLines(string documentType, string? documentReasonCode, IReadOnlyList<InventoryDocumentLineForm> lines)
    {
        var type = (documentType ?? string.Empty).Trim().ToLowerInvariant();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line.QualityStatusId))
                return "برای همه آیتم‌ها وضعیت کیفی را انتخاب کنید.";

            if (type is "issue" && string.IsNullOrWhiteSpace(line.SourceLocationId))
                return "برای سند خروج، لوکیشن مبدا همه آیتم‌ها الزامی است.";

            if (type is "transfer" && (string.IsNullOrWhiteSpace(line.SourceLocationId) || string.IsNullOrWhiteSpace(line.DestinationLocationId)))
                return "برای سند انتقال، لوکیشن مبدا و مقصد همه آیتم‌ها الزامی است.";

            if (type is "adjustment")
            {
                if (string.Equals(line.AdjustmentDirection, "Decrease", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(line.SourceLocationId))
                    return "برای تعدیل کاهشی، لوکیشن مبدا همه آیتم‌ها الزامی است.";

                if (!string.Equals(line.AdjustmentDirection, "Decrease", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(line.DestinationLocationId))
                    return "برای تعدیل افزایشی، لوکیشن مقصد همه آیتم‌ها الزامی است.";

                if (string.IsNullOrWhiteSpace(line.ReasonCode) && string.IsNullOrWhiteSpace(documentReasonCode))
                    return "برای سند تعدیل، کد علت الزامی است.";
            }

            if (type is not "issue" and not "transfer" and not "adjustment" && string.IsNullOrWhiteSpace(line.DestinationLocationId))
                return "برای سند ورود، لوکیشن مقصد همه آیتم‌ها الزامی است.";
        }

        return null;
    }

    private bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Token") ?? string.Empty;
        return !string.IsNullOrWhiteSpace(token);
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
                    return roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }
            catch
            {
            }
        }

        var extractedRoles = JwtRoleExtractor.ExtractRoles(token).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(extractedRoles));
        return extractedRoles;
    }

    private IReadOnlyList<string> ResolvePermissionsFromSession()
    {
        var permissionsJson = HttpContext.Session.GetString("Permissions");
        if (string.IsNullOrWhiteSpace(permissionsJson))
            return Array.Empty<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private void SetLayoutViewBag(IReadOnlyList<DashboardMenuModule> modules, string? activeModuleId, string? activeItemId, string userName)
    {
        ViewBag.UserDisplayName = userName;
        ViewBag.MenuModules = modules;
        ViewBag.ActiveModuleId = activeModuleId;
        ViewBag.ActiveItemId = activeItemId;
    }

}
