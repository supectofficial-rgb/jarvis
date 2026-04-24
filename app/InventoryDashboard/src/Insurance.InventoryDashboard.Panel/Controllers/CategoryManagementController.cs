using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class CategoryManagementController : CatalogManagementController
{
    public CategoryManagementController(ICatalogApiService apiService, IDashboardConfigService dashboardConfigService)
        : base(apiService, dashboardConfigService)
    {
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Categories));

    [HttpGet]
    public new Task<IActionResult> Categories(
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => base.Categories(categoryId, searchTerm, statusFilter, sort, page, pageSize, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveCategory(CategoryUpsertForm form)
        => base.SaveCategory(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> MoveCategory(MoveCategoryForm form)
        => base.MoveCategory(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateCategory(string categoryId)
        => base.ActivateCategory(categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateCategory(string categoryId)
        => base.DeactivateCategory(categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeleteCategory(string categoryId)
        => base.DeleteCategory(categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ApplyCategoryBulkAction(
        string selectedIds,
        string bulkAction,
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
        => base.ApplyCategoryBulkAction(selectedIds, bulkAction, categoryId, searchTerm, statusFilter, sort, page, pageSize);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> CreateCategoryAttribute(AttributeDefinitionForm form)
        => base.CreateCategoryAttribute(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpdateAttributeDefinition(AttributeDefinitionUpdateForm form)
        => base.UpdateAttributeDefinition(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> AddAttributeOption(AttributeOptionForm form)
        => base.AddAttributeOption(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpdateAttributeOption(AttributeOptionUpdateForm form)
        => base.UpdateAttributeOption(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateAttributeOption(string categoryId, string attributeId, string optionId)
        => base.ActivateAttributeOption(categoryId, attributeId, optionId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateAttributeOption(string categoryId, string attributeId, string optionId)
        => base.DeactivateAttributeOption(categoryId, attributeId, optionId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeleteAttributeOption(string categoryId, string attributeId, string optionId)
        => base.DeleteAttributeOption(categoryId, attributeId, optionId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> AssignAttributeToCategory(AssignAttributeForm form)
        => base.AssignAttributeToCategory(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpdateCategoryAttributeRule(CategoryAttributeRuleForm form)
        => base.UpdateCategoryAttributeRule(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateCategoryAttributeRule(string categoryId, string attributeId)
        => base.ActivateCategoryAttributeRule(categoryId, attributeId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateCategoryAttributeRule(string categoryId, string attributeId)
        => base.DeactivateCategoryAttributeRule(categoryId, attributeId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveCategoryAttributeRule(string categoryId, string attributeId)
        => base.RemoveCategoryAttributeRule(categoryId, attributeId);
}
