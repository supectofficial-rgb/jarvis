using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class VariantManagementController : CatalogManagementController
{
    public VariantManagementController(ICatalogApiService apiService, IDashboardConfigService dashboardConfigService)
        : base(apiService, dashboardConfigService)
    {
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Variants));

    [HttpGet]
    public new Task<IActionResult> Variants(
        string? productId,
        string? variantId,
        string? searchTerm,
        string? trackingPolicy,
        string? statusFilter,
        string? attributeTypeFilter,
        string? sort,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => base.Variants(productId, variantId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveVariant(VariantUpsertForm form)
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
    public new Task<IActionResult> SetVariantAttributeValue(VariantAttributeValueForm form)
        => base.SetVariantAttributeValue(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantAttributeValue(string productId, string variantId, string attributeId)
        => base.RemoveVariantAttributeValue(productId, variantId, attributeId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpsertVariantUomConversion(VariantUomConversionForm form)
        => base.UpsertVariantUomConversion(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantUomConversion(string productId, string variantId, string fromUomRef, string toUomRef)
        => base.RemoveVariantUomConversion(productId, variantId, fromUomRef, toUomRef);
}
