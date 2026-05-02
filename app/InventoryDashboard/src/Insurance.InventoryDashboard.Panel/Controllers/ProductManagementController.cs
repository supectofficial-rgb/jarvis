using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class ProductManagementController : CatalogManagementController
{
    public ProductManagementController(
        ICatalogApiService apiService,
        IDashboardConfigService dashboardConfigService,
        ILogger<CatalogManagementController> logger)
        : base(apiService, dashboardConfigService, logger)
    {
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Products));

    [HttpGet]
    public new Task<IActionResult> Products(
        string? categoryId,
        string? productId,
        string? searchTerm,
        string? categoryFilterId,
        string? statusFilter,
        string? sort,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => base.Products(categoryId, productId, searchTerm, categoryFilterId, statusFilter, sort, createNew, page, pageSize, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveProduct([Bind(Prefix = "ProductForm")] ProductUpsertForm form)
        => base.SaveProduct(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateProduct(string productId, string? categoryId)
        => base.ActivateProduct(productId, categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateProduct(string productId, string? categoryId)
        => base.DeactivateProduct(productId, categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeleteProduct(string productId, string? categoryId)
        => base.DeleteProduct(productId, categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ApplyProductBulkAction(
        string selectedIds,
        string bulkAction,
        string? categoryId,
        string? searchTerm,
        string? categoryFilterId,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
        => base.ApplyProductBulkAction(selectedIds, bulkAction, categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ChangeProductCategory([Bind(Prefix = "ProductCategoryChangeForm")] ProductCategoryChangeForm form)
        => base.ChangeProductCategory(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SetProductAttributeValue([Bind(Prefix = "ProductAttributeForm")] ProductAttributeValueForm form)
        => base.SetProductAttributeValue(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveProductAttributeValue(string productId, string categoryId, string attributeId)
        => base.RemoveProductAttributeValue(productId, categoryId, attributeId);
}
