using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class VariantManagementController : CatalogManagementController
{
    public VariantManagementController(
        ICatalogApiService apiService,
        IDashboardConfigService dashboardConfigService,
        ILogger<CatalogManagementController> logger)
        : base(apiService, dashboardConfigService, logger)
    {
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Variants));

    [HttpGet]
    public new async Task<IActionResult> Variants(
        string? productId,
        string? variantId,
        string? categoryId,
        string? searchTerm,
        string? attributeOptionIds,
        string? trackingPolicy,
        string? statusFilter,
        string? attributeTypeFilter,
        string? sort,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await base.Variants(productId, variantId, categoryId, searchTerm, attributeOptionIds, trackingPolicy, statusFilter, attributeTypeFilter, sort, createNew, page, pageSize, cancellationToken);
        if (result is not ViewResult { Model: VariantManagementPageViewModel model } viewResult)
        {
            return result;
        }

        if (!TryGetToken(out var token))
        {
            return result;
        }

        var relatedVariantsResult = await _apiService.SearchProductVariantsAsync(token, isActive: true, pageSize: 2000);
        var relatedVariants = (relatedVariantsResult.Data?.Items ?? new List<ProductVariantSummaryModel>())
            .Where(x => !string.Equals(x.Id, model.SelectedVariantId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Sku)
            .ToList();

        model.RelatedVariantLookup = relatedVariants;
        model.VariantComponents = model.SelectedVariantDetails?.Components ?? new List<VariantComponentModel>();
        model.VariantAddOns = model.SelectedVariantDetails?.AddOns ?? new List<VariantAddOnModel>();
        model.VariantComponentForm = new VariantComponentForm
        {
            ProductId = model.SelectedProductId ?? string.Empty,
            VariantId = model.SelectedVariantId ?? string.Empty,
            Quantity = 1m
        };
        model.VariantAddOnForm = new VariantAddOnForm
        {
            ProductId = model.SelectedProductId ?? string.Empty,
            VariantId = model.SelectedVariantId ?? string.Empty
        };
        model.ErrorMessage = string.Join(" | ", new[] { model.ErrorMessage, relatedVariantsResult.ErrorMessage }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return viewResult;
    }

    [HttpGet]
    public async Task<IActionResult> VariantStocks(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(variantId, out var variantRef))
        {
            return BadRequest(new { error = "شناسه واریانت معتبر نیست." });
        }

        var result = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: variantRef);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data?.Items ?? new List<StockDetailBucketModel>());
    }

    [HttpGet]
    public async Task<IActionResult> VariantPrices(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(variantId, out var variantRef))
        {
            return BadRequest(new { error = "شناسه واریانت معتبر نیست." });
        }

        var result = await _apiService.SearchSellerVariantPricesAsync(token, variantRef: variantRef, pageSize: 100);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data?.Items ?? new List<SellerVariantPriceModel>());
    }

    [HttpGet]
    public async Task<IActionResult> VariantDocuments(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        var result = await _apiService.GetInventoryTransactionsByVariantAsync(variantId, token);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data ?? new List<InventoryTransactionListItemModel>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveVariant([Bind(Prefix = "VariantForm")] VariantUpsertForm form)
        => base.SaveVariant(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateVariant(string productId, string variantId)
        => base.ActivateVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateVariant(string productId, string variantId)
        => base.DeactivateVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeleteVariant(string productId, string variantId)
        => base.DeleteVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ApplyVariantBulkAction(
        string selectedIds,
        string bulkAction,
        string? productId,
        string? searchTerm,
        string? trackingPolicy,
        string? statusFilter,
        string? attributeTypeFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
        => base.ApplyVariantBulkAction(selectedIds, bulkAction, productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> LockVariantInventoryMovement(string productId, string variantId)
        => base.LockVariantInventoryMovement(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ChangeVariantTrackingPolicy(string productId, string variantId, string trackingPolicy)
        => base.ChangeVariantTrackingPolicy(productId, variantId, trackingPolicy);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ChangeVariantBaseUom(string productId, string variantId, string baseUomRef)
        => base.ChangeVariantBaseUom(productId, variantId, baseUomRef);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SetVariantAttributeValue([Bind(Prefix = "VariantAttributeForm")] VariantAttributeValueForm form)
        => base.SetVariantAttributeValue(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantAttributeValue(string productId, string variantId, string attributeId)
        => base.RemoveVariantAttributeValue(productId, variantId, attributeId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpsertVariantUomConversion([Bind(Prefix = "VariantUomConversionForm")] VariantUomConversionForm form)
        => base.UpsertVariantUomConversion(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantUomConversion(string productId, string variantId, string fromUomRef, string toUomRef)
        => base.RemoveVariantUomConversion(productId, variantId, fromUomRef, toUomRef);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertVariantComponent([Bind(Prefix = "VariantComponentForm")] VariantComponentForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var result = await _apiService.UpsertVariantComponentAsync(
            form.VariantId,
            new UpsertVariantComponentRequest
            {
                ComponentVariantRef = form.ComponentVariantId,
                Quantity = form.Quantity
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "ثبت جزء واریانت انجام نشد.";
        }
        else
        {
            TempData["CatalogSuccess"] = "جزء/معادل واریانت با موفقیت ذخیره شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantComponent(string productId, string variantId, string componentVariantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.RemoveVariantComponentAsync(variantId, componentVariantId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "حذف جزء واریانت انجام نشد.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertVariantAddOn([Bind(Prefix = "VariantAddOnForm")] VariantAddOnForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var result = await _apiService.UpsertVariantAddOnAsync(
            form.VariantId,
            new UpsertVariantAddOnRequest
            {
                AddOnVariantRef = form.AddOnVariantId
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "ثبت Add-on واریانت انجام نشد.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Add-on واریانت با موفقیت ذخیره شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantAddOn(string productId, string variantId, string addOnVariantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.RemoveVariantAddOnAsync(variantId, addOnVariantId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "حذف Add-on واریانت انجام نشد.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }
}
