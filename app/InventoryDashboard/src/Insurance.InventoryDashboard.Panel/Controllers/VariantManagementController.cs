using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class VariantManagementController : CatalogManagementController
{
    private readonly IApiService _inventoryApiService;

    public VariantManagementController(
        IApiService inventoryApiService,
        ICatalogApiService apiService,
        IDashboardConfigService dashboardConfigService,
        ILogger<CatalogManagementController> logger)
        : base(apiService, dashboardConfigService, logger)
    {
        _inventoryApiService = inventoryApiService;
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
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Sku)
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
        model.VariantAssemblyOperationForm = new VariantAssemblyOperationForm
        {
            ProductId = model.SelectedProductId ?? string.Empty,
            VariantId = model.SelectedVariantId ?? string.Empty,
            OperationType = "Assemble",
            Quantity = 1m
        };
        model.BulkVariantAddOnForm = new BulkVariantAddOnForm
        {
            ProductId = model.SelectedProductId
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
            return BadRequest(new { error = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª." });
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
            return BadRequest(new { error = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª." });
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
                ComponentId = form.ComponentId,
                ComponentVariantRef = form.ComponentVariantId,
                WarehouseRef = form.WarehouseId,
                LocationRef = form.LocationId,
                Quantity = form.Quantity
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â¬Ã˜Â²Ã˜Â¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Ã˜Â¬Ã˜Â²Ã˜Â¡/Ã™â€¦Ã˜Â¹Ã˜Â§Ã˜Â¯Ã™â€ž Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â°Ã˜Â®Ã›Å’Ã˜Â±Ã™â€¡ Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantComponent(string productId, string variantId, string componentVariantId, string? componentId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.RemoveVariantComponentAsync(variantId, componentVariantId, componentId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Ã˜Â¬Ã˜Â²Ã˜Â¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
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
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â°Ã˜Â®Ã›Å’Ã˜Â±Ã™â€¡ Ã˜Â´Ã˜Â¯.";
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
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteVariantAssembly([Bind(Prefix = "VariantAssemblyOperationForm")] VariantAssemblyOperationForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(
                token,
                "Inventory.Document.Create",
                "InventoryDocument.Create",
                "Document.Create",
                "Inventory.Document.Approve",
                "InventoryDocument.Approve",
                "Document.Approve",
                "Inventory.Document.Post",
                "InventoryDocument.Post",
                "Document.Post"))
        {
            TempData["CatalogError"] = "Ã˜Â¯Ã˜Â³Ã˜ÂªÃ˜Â±Ã˜Â³Ã›Å’ Ã™â€žÃ˜Â§Ã˜Â²Ã™â€¦ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â«Ã˜Â¨Ã˜Âª Ã™Ë† Ã™Â¾Ã˜Â³Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â±Ã˜Â§ Ã™â€ Ã˜Â¯Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var detailsResult = await _apiService.GetProductVariantFullDetailsAsync(form.VariantId, token);
        var details = detailsResult.Data;
        if (!detailsResult.IsSuccess || details is null)
        {
            TempData["CatalogError"] = detailsResult.ErrorMessage ?? "Ã˜Â¬Ã˜Â²Ã˜Â¦Ã›Å’Ã˜Â§Ã˜Âª Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¹Ã™â€¦Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var recipe = details.Components
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.ComponentVariantId) &&
                !string.IsNullOrWhiteSpace(x.WarehouseId) &&
                !string.IsNullOrWhiteSpace(x.LocationId) &&
                x.Quantity > 0)
            .ToList();

        if (recipe.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª recipe Ã›Å’Ã˜Â§ Ã™â€žÃ›Å’Ã˜Â³Ã˜Âª Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â«Ã˜Â¨Ã˜Âª Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â§Ã˜Â³Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var operationIsAssemble = string.Equals(form.OperationType, "Assemble", StringComparison.OrdinalIgnoreCase);
        var contextResult = operationIsAssemble
            ? await BuildAssembleDocumentAsync(token, details, recipe, form.Quantity, form.ReasonCode)
            : await BuildDisassembleDocumentAsync(token, details, recipe, form.Quantity, form.ReasonCode);

        if (!contextResult.Success || contextResult.Form is null)
        {
            TempData["CatalogError"] = contextResult.Error ?? "Ã˜Â§Ã™â€¦ÃšÂ©Ã˜Â§Ã™â€  Ã˜Â¢Ã™â€¦Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯ Ã™â€ Ã˜Â¯Ã˜Â§Ã˜Â´Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var createResult = await _inventoryApiService.CreateInventoryDocumentAsync(contextResult.Form, token);
        if (!createResult.IsSuccess || createResult.Data is null)
        {
            TempData["CatalogError"] = createResult.ErrorMessage ?? "Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var approvedBy = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var approveResult = await _inventoryApiService.ApproveInventoryDocumentAsync(createResult.Data.DocumentId, approvedBy, token);
        if (!approveResult.IsSuccess)
        {
            TempData["CatalogError"] = approveResult.ErrorMessage ?? "Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã˜Â´Ã˜Â¯ Ã˜Â§Ã™â€¦Ã˜Â§ Ã˜ÂªÃ˜Â§Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â¢Ã™â€  Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var postResult = await _inventoryApiService.PostInventoryDocumentAsync(createResult.Data.DocumentId, approvedBy, token);
        if (!postResult.IsSuccess)
        {
            TempData["CatalogError"] = postResult.ErrorMessage ?? "Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜ÂªÃ˜Â§Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯ Ã˜Â§Ã™â€¦Ã˜Â§ Ã™Â¾Ã˜Â³Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â¢Ã™â€  Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        TempData["CatalogSuccess"] = $"{(operationIsAssemble ? "Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ" : "Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž")} Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã˜Â´Ã˜Â¯ Ã™Ë† Ã˜Â³Ã™â€ Ã˜Â¯ {createResult.Data.DocumentNo} Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯.";
        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAssignVariantAddOn([Bind(Prefix = "BulkVariantAddOnForm")] BulkVariantAddOnForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var selectedVariantIds = (form.SelectedVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var selectedAddOnIds = (form.SelectedAddOnVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedAddOnIds.Count == 0 && !string.IsNullOrWhiteSpace(form.AddOnVariantId))
            selectedAddOnIds.Add(form.AddOnVariantId);

        if (selectedVariantIds.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â­Ã˜Â¯Ã˜Â§Ã™â€šÃ™â€ž Ã›Å’ÃšÂ© Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â§Ã˜Â¨ ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        if (selectedAddOnIds.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â­Ã˜Â¯Ã˜Â§Ã™â€šÃ™â€ž Ã›Å’ÃšÂ© Add-on Ã˜Â±Ã˜Â§ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â³Ã˜Â§Ã˜Â¨ Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â§Ã˜Â¨ ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var failures = new List<string>();
        var successCount = 0;

        foreach (var variantId in selectedVariantIds)
        {
            if (selectedAddOnIds.Count > 0)
            {
                var variantFailed = false;
                foreach (var addOnVariantId in selectedAddOnIds)
                {
                    if (string.Equals(variantId, addOnVariantId, StringComparison.OrdinalIgnoreCase))
                    {
                        failures.Add($"Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜ÂªÃ™Ë†Ã˜Â§Ã™â€ Ã˜Â¯ Add-on Ã˜Â®Ã™Ë†Ã˜Â¯Ã˜Â´ Ã˜Â¨Ã˜Â§Ã˜Â´Ã˜Â¯.");
                        variantFailed = true;
                        break;
                    }

                    var addOnResult = await _apiService.UpsertVariantAddOnAsync(
                        variantId,
                        new UpsertVariantAddOnRequest
                        {
                            AddOnVariantRef = addOnVariantId
                        },
                        token);

                    if (addOnResult.IsSuccess)
                        continue;

                    failures.Add(addOnResult.ErrorMessage ?? $"Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.");
                    variantFailed = true;
                    break;
                }

                if (!variantFailed)
                    successCount++;

                continue;
            }
            if (string.Equals(variantId, form.AddOnVariantId, StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜ÂªÃ™Ë†Ã˜Â§Ã™â€ Ã˜Â¯ Add-on Ã˜Â®Ã™Ë†Ã˜Â¯Ã˜Â´ Ã˜Â¨Ã˜Â§Ã˜Â´Ã˜Â¯.");
                continue;
            }

            var result = await _apiService.UpsertVariantAddOnAsync(
                variantId,
                new UpsertVariantAddOnRequest
                {
                    AddOnVariantRef = form.AddOnVariantId
                },
                token);

            if (!result.IsSuccess)
            {
                failures.Add(result.ErrorMessage ?? $"Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.");
                continue;
            }

            successCount++;
        }

        if (successCount > 0)
        {
            TempData["CatalogSuccess"] = $"Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ {successCount} Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯.";
        }

        if (failures.Count > 0)
        {
            TempData["CatalogError"] = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty);
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = selectedVariantIds.FirstOrDefault() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAssignVariantImages([Bind(Prefix = "BulkVariantImageForm")] BulkVariantImageForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var selectedVariantIds = (form.SelectedVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedVariantIds.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â­Ã˜Â¯Ã˜Â§Ã™â€šÃ™â€ž Ã›Å’ÃšÂ© Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â§Ã˜Â¨ ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        List<VariantImageModel> images;
        try
        {
            images = JsonSerializer.Deserialize<List<VariantImageModel>>(form.ImagesJson ?? "[]", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<VariantImageModel>();
        }
        catch
        {
            TempData["CatalogError"] = "Ã˜Â³Ã˜Â§Ã˜Â®Ã˜ÂªÃ˜Â§Ã˜Â± Ã˜ÂªÃ˜ÂµÃ˜Â§Ã™Ë†Ã›Å’Ã˜Â± Ã˜Â§Ã˜Â±Ã˜Â³Ã˜Â§Ã™â€žÃ›Å’ Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        images = images
            .Where(x => !string.IsNullOrWhiteSpace(x.FileKey))
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        if (images.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã˜Â­Ã˜Â¯Ã˜Â§Ã™â€šÃ™â€ž Ã›Å’ÃšÂ© Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã˜Â¢Ã™Â¾Ã™â€žÃ™Ë†Ã˜Â¯ ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var failures = new List<string>();
        var successCount = 0;

        foreach (var variantId in selectedVariantIds)
        {
            var variantFailed = false;
            foreach (var image in images)
            {
                var result = await _apiService.UpsertVariantImageAsync(
                    variantId,
                    new UpsertVariantImageRequest
                    {
                        FileKey = image.FileKey,
                        OriginalFileName = image.OriginalFileName,
                        ContentType = image.ContentType,
                        OriginalUrl = image.OriginalUrl,
                        ThumbnailUrl = image.ThumbnailUrl,
                        DisplayOrder = image.DisplayOrder,
                        IsPrimary = image.IsPrimary
                    },
                    token);

                if (result.IsSuccess)
                    continue;

                variantFailed = true;
                failures.Add(result.ErrorMessage ?? $"Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.");
                break;
            }

            if (!variantFailed)
                successCount++;
        }

        if (successCount > 0)
            TempData["CatalogSuccess"] = $"Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ {successCount} Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯.";

        if (failures.Count > 0)
            TempData["CatalogError"] = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty);

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = selectedVariantIds.FirstOrDefault() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantImage(string productId, string variantId, string fileKey)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(variantId) || string.IsNullOrWhiteSpace(fileKey))
        {
            TempData["CatalogError"] = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã›Å’Ã˜Â§ Ã™ÂÃ˜Â§Ã›Å’Ã™â€ž Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.RemoveVariantImageAsync(variantId, fileKey, token);
        if (!result.IsSuccess)
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        else
            TempData["CatalogSuccess"] = "Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â­Ã˜Â°Ã™Â Ã˜Â´Ã˜Â¯.";

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildAssembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (recipe.Count > 0)
        {
            if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
                return (false, warehouseError, null);

            var recipeLocations = recipe
                .Select(x => x.LocationId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (recipeLocations.Count != 1)
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Å’ Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ recipe Ã˜Â¨Ã˜Â§Ã›Å’Ã˜Â¯ Ã›Å’ÃšÂ© Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€  Ã™â€¦Ã™â€šÃ˜ÂµÃ˜Â¯ Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¯Ã˜Â§Ã˜Â´Ã˜ÂªÃ™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â´Ã™â€ Ã˜Â¯.", null);

            var componentBuckets = new Dictionary<string, List<StockDetailBucketModel>>(StringComparer.OrdinalIgnoreCase);
            foreach (var component in recipe)
            {
                var componentVariantRef = Guid.Parse(component.ComponentVariantId);
                var configuredLocationRef = Guid.Parse(component.LocationId);
                var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: componentVariantRef);
                if (!bucketsResult.IsSuccess)
                    return (false, bucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);

                componentBuckets[component.ComponentId] = (bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                    .Where(x =>
                        x.QuantityOnHand > 0 &&
                        x.WarehouseRef == recipeWarehouseRef &&
                        x.LocationRef == configuredLocationRef)
                    .ToList();
            }

            var sellerQualityGroups = recipe.Select(component =>
                componentBuckets[component.ComponentId]
                    .GroupBy(ToSellerQualityKey)
                    .Where(group => group.Sum(x => x.QuantityOnHand) >= component.Quantity * quantity)
                    .Select(group => group.Key)
                    .ToHashSet())
                .ToList();

            if (sellerQualityGroups.Any(group => group.Count == 0))
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã›Å’ÃšÂ©Ã›Å’ Ã˜Â§Ã˜Â² Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã™Ë† Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€  Ã˜Â«Ã˜Â¨Ã˜ÂªÃ¢â‚¬Å’Ã˜Â´Ã˜Â¯Ã™â€¡ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);

            var sharedSellerQualities = sellerQualityGroups.Skip(1)
                .Aggregate(new HashSet<string>(sellerQualityGroups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
                {
                    acc.IntersectWith(set);
                    return acc;
                });

            if (sharedSellerQualities.Count == 0)
                return (false, "Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€ Ã˜Â¯.", null);

            if (sharedSellerQualities.Count > 1)
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);

            var selectedSellerQuality = sharedSellerQualities.First();
            var selectedSellerQualityParts = ParseSellerQualityKey(selectedSellerQuality);
            var assembledLines = new List<CreateInventoryDocumentLineForm>();

            foreach (var component in recipe)
            {
                var remaining = component.Quantity * quantity;
                foreach (var bucket in componentBuckets[component.ComponentId]
                             .Where(x => string.Equals(ToSellerQualityKey(x), selectedSellerQuality, StringComparison.OrdinalIgnoreCase))
                             .OrderByDescending(x => x.QuantityOnHand))
                {
                    if (remaining <= 0)
                        break;

                    var take = Math.Min(bucket.QuantityOnHand, remaining);
                    if (take <= 0)
                        continue;

                    assembledLines.Add(new CreateInventoryDocumentLineForm
                    {
                        VariantId = component.ComponentVariantId,
                        Qty = take,
                        UomRef = string.Empty,
                        BaseUomRef = string.Empty,
                        SourceLocationRef = bucket.LocationRef.ToString("D"),
                        QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                        LotBatchNo = bucket.LotBatchNo
                    });

                    remaining -= take;
                }

                if (remaining > 0)
                    return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            assembledLines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = variant.Id,
                Qty = quantity,
                UomRef = variant.BaseUomRef,
                BaseUomRef = variant.BaseUomRef,
                DestinationLocationRef = recipeLocations[0],
                QualityStatusRef = selectedSellerQualityParts.QualityStatusRef.ToString("D")
            });

            return (true, null, CreateConversionDocumentForm(
                recipeWarehouseRef,
                selectedSellerQualityParts.SellerRef,
                $"Assembly:{variant.Sku}",
                reasonCode,
                assembledLines));
        }

        var requiredByComponent = recipe.ToDictionary(
            x => x.ComponentVariantId,
            x => x.Quantity * quantity,
            StringComparer.OrdinalIgnoreCase);

        var bucketMap = new Dictionary<string, List<StockDetailBucketModel>>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in requiredByComponent)
        {
            var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: Guid.Parse(entry.Key));
            if (!bucketsResult.IsSuccess)
            {
                return (false, bucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            var buckets = bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>();
            bucketMap[entry.Key] = buckets.Where(x => x.QuantityOnHand > 0).ToList();
        }

        var contextGroups = requiredByComponent.Select(entry =>
            bucketMap[entry.Key]
                .GroupBy(ToContextKey)
                .Where(group => group.Sum(x => x.QuantityOnHand) >= entry.Value)
                .Select(group => group.Key)
                .ToHashSet())
            .ToList();

        if (contextGroups.Any(group => group.Count == 0))
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã›Å’ÃšÂ©Ã›Å’ Ã˜Â§Ã˜Â² Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â¯Ã˜Â± Ã™â€¡Ã›Å’Ãšâ€  context Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ©Ã›Å’ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        var sharedContexts = contextGroups.Skip(1)
            .Aggregate(new HashSet<string>(contextGroups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
            {
                acc.IntersectWith(set);
                return acc;
            });

        if (sharedContexts.Count == 0)
        {
            return (false, "Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±/Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€ /ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€ Ã˜Â¯.", null);
        }

        if (sharedContexts.Count > 1)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© context Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
        }

        var selectedContext = sharedContexts.First();
        var contextParts = ParseContextKey(selectedContext);
        var lines = new List<CreateInventoryDocumentLineForm>();

        foreach (var entry in requiredByComponent)
        {
            var remaining = entry.Value;
            foreach (var bucket in bucketMap[entry.Key]
                         .Where(x => string.Equals(ToContextKey(x), selectedContext, StringComparison.OrdinalIgnoreCase))
                         .OrderByDescending(x => x.QuantityOnHand))
            {
                if (remaining <= 0)
                {
                    break;
                }

                var take = Math.Min(bucket.QuantityOnHand, remaining);
                if (take <= 0)
                {
                    continue;
                }

                lines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = entry.Key,
                    Qty = take,
                    UomRef = string.Empty,
                    BaseUomRef = string.Empty,
                    SourceLocationRef = bucket.LocationRef.ToString("D"),
                    QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                    LotBatchNo = bucket.LotBatchNo
                });

                remaining -= take;
            }

            if (remaining > 0)
            {
                return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }
        }

        lines.Add(new CreateInventoryDocumentLineForm
        {
            VariantId = variant.Id,
            Qty = quantity,
            UomRef = variant.BaseUomRef,
            BaseUomRef = variant.BaseUomRef,
            DestinationLocationRef = contextParts.LocationRef.ToString("D"),
            QualityStatusRef = contextParts.QualityStatusRef.ToString("D")
        });

        return (true, null, CreateConversionDocumentForm(
            contextParts.WarehouseRef,
            contextParts.SellerRef,
            $"Assembly:{variant.Sku}",
            reasonCode,
            lines));
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildDisassembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (recipe.Count > 0)
        {
            if (!Guid.TryParse(variant.Id, out var recipeVariantRef))
            {
                return (false, "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
            {
                return (false, warehouseError, null);
            }

            var recipeBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: recipeVariantRef);
            if (!recipeBucketsResult.IsSuccess)
            {
                return (false, recipeBucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            var recipeCandidateBuckets = (recipeBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                .Where(x => x.QuantityOnHand > 0 && x.WarehouseRef == recipeWarehouseRef)
                .ToList();

            var candidateSellerQualities = recipeCandidateBuckets
                .GroupBy(ToSellerQualityKey)
                .Where(group => group.Sum(x => x.QuantityOnHand) >= quantity)
                .Select(group => group.Key)
                .ToList();

            if (candidateSellerQualities.Count == 0)
            {
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€žÃ˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± recipe Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            if (candidateSellerQualities.Count > 1)
            {
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
            }

            var selectedSellerQuality = candidateSellerQualities[0];
            var selectedSellerQualityParts = ParseSellerQualityKey(selectedSellerQuality);
            var disassemblyLines = new List<CreateInventoryDocumentLineForm>();
            var remainingRecipeQty = quantity;

            foreach (var bucket in recipeCandidateBuckets
                         .Where(x => string.Equals(ToSellerQualityKey(x), selectedSellerQuality, StringComparison.OrdinalIgnoreCase))
                         .OrderByDescending(x => x.QuantityOnHand))
            {
                if (remainingRecipeQty <= 0)
                {
                    break;
                }

                var take = Math.Min(bucket.QuantityOnHand, remainingRecipeQty);
                if (take <= 0)
                {
                    continue;
                }

                disassemblyLines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = variant.Id,
                    Qty = take,
                    UomRef = variant.BaseUomRef,
                    BaseUomRef = variant.BaseUomRef,
                    SourceLocationRef = bucket.LocationRef.ToString("D"),
                    QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                    LotBatchNo = bucket.LotBatchNo
                });

                remainingRecipeQty -= take;
            }

            if (remainingRecipeQty > 0)
            {
                return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            foreach (var component in recipe)
            {
                disassemblyLines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = component.ComponentVariantId,
                    Qty = component.Quantity * quantity,
                    UomRef = string.Empty,
                    BaseUomRef = string.Empty,
                    DestinationLocationRef = component.LocationId,
                    QualityStatusRef = selectedSellerQualityParts.QualityStatusRef.ToString("D")
                });
            }

            return (true, null, CreateConversionDocumentForm(
                recipeWarehouseRef,
                selectedSellerQualityParts.SellerRef,
                $"Disassembly:{variant.Sku}",
                reasonCode,
                disassemblyLines));
        }

        if (!Guid.TryParse(variant.Id, out var variantRef))
        {
            return (false, "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
        }

        var bucketsResultFallback = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: variantRef);
        if (!bucketsResultFallback.IsSuccess)
        {
            return (false, bucketsResultFallback.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        var candidateBucketsFallback = (bucketsResultFallback.Data?.Items ?? new List<StockDetailBucketModel>())
            .Where(x => x.QuantityOnHand > 0)
            .ToList();

        var candidateContexts = candidateBucketsFallback
            .GroupBy(ToContextKey)
            .Where(group => group.Sum(x => x.QuantityOnHand) >= quantity)
            .Select(group => group.Key)
            .ToList();

        if (candidateContexts.Count == 0)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€žÃ˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        if (candidateContexts.Count > 1)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© context Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
        }

        var selectedContext = candidateContexts[0];
        var contextParts = ParseContextKey(selectedContext);
        var lines = new List<CreateInventoryDocumentLineForm>();
        var remaining = quantity;

        foreach (var bucket in candidateBucketsFallback
                     .Where(x => string.Equals(ToContextKey(x), selectedContext, StringComparison.OrdinalIgnoreCase))
                     .OrderByDescending(x => x.QuantityOnHand))
        {
            if (remaining <= 0)
            {
                break;
            }

            var take = Math.Min(bucket.QuantityOnHand, remaining);
            if (take <= 0)
            {
                continue;
            }

            lines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = variant.Id,
                Qty = take,
                UomRef = variant.BaseUomRef,
                BaseUomRef = variant.BaseUomRef,
                SourceLocationRef = bucket.LocationRef.ToString("D"),
                QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                LotBatchNo = bucket.LotBatchNo
            });

            remaining -= take;
        }

        if (remaining > 0)
        {
            return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
        }

        foreach (var component in recipe)
        {
            var componentQty = component.Quantity * quantity;
            lines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = component.ComponentVariantId,
                Qty = componentQty,
                UomRef = string.Empty,
                BaseUomRef = string.Empty,
                DestinationLocationRef = contextParts.LocationRef.ToString("D"),
                QualityStatusRef = contextParts.QualityStatusRef.ToString("D")
            });
        }

        return (true, null, CreateConversionDocumentForm(
            contextParts.WarehouseRef,
            contextParts.SellerRef,
            $"Disassembly:{variant.Sku}",
            reasonCode,
            lines));
    }

    private static CreateInventoryDocumentForm CreateConversionDocumentForm(
        Guid warehouseRef,
        Guid sellerRef,
        string referenceType,
        string? reasonCode,
        List<CreateInventoryDocumentLineForm> lines)
    {
        return new CreateInventoryDocumentForm
        {
            DocumentType = "Conversion",
            WarehouseRef = warehouseRef.ToString("D"),
            SellerRef = sellerRef.ToString("D"),
            OccurredAt = DateTime.Now,
            ReferenceType = referenceType,
            ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim(),
            Lines = lines
        };
    }

    private static string ToSellerQualityKey(StockDetailBucketModel bucket)
        => $"{bucket.SellerRef:D}|{bucket.QualityStatusRef:D}";

    private static (Guid SellerRef, Guid QualityStatusRef) ParseSellerQualityKey(string key)
    {
        var parts = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (
            Guid.Parse(parts[0]),
            Guid.Parse(parts[1]));
    }

    private static bool TryResolveRecipeWarehouse(
        IReadOnlyCollection<VariantComponentModel> recipe,
        out Guid warehouseRef,
        out string? error)
    {
        warehouseRef = Guid.Empty;
        error = null;

        var warehouseIds = recipe
            .Select(x => x.WarehouseId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (warehouseIds.Count != 1)
        {
            error = "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¹Ã™â€¦Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Å’ Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ recipe Ã˜Â¨Ã˜Â§Ã›Å’Ã˜Â¯ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â´Ã™â€ Ã˜Â¯.";
            return false;
        }

        if (!Guid.TryParse(warehouseIds[0], out warehouseRef))
        {
            error = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± recipe Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.";
            return false;
        }

        return true;
    }

    private static string ToContextKey(StockDetailBucketModel bucket)
        => $"{bucket.SellerRef:D}|{bucket.WarehouseRef:D}|{bucket.LocationRef:D}|{bucket.QualityStatusRef:D}";

    private static (Guid SellerRef, Guid WarehouseRef, Guid LocationRef, Guid QualityStatusRef) ParseContextKey(string key)
    {
        var parts = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (
            Guid.Parse(parts[0]),
            Guid.Parse(parts[1]),
            Guid.Parse(parts[2]),
            Guid.Parse(parts[3]));
    }
}
