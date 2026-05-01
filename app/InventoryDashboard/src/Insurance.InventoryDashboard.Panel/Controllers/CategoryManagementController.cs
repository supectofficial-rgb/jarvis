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
    public Task<IActionResult> Categories(
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        bool createNew = false,
        bool clearCatalogMessage = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (clearCatalogMessage)
        {
            TempData.Remove("CatalogError");
            TempData.Remove("CatalogSuccess");
        }

        ModelState.Clear();
        return base.Categories(categoryId, searchTerm, statusFilter, sort, createNew, page, pageSize, "categories", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> Attributes(
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => base.Categories(categoryId, searchTerm, statusFilter, sort, false, page, pageSize, "attributes", cancellationToken);

    [HttpGet]
    public Task<IActionResult> CategoryAttributes(
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => base.Categories(categoryId, searchTerm, statusFilter, sort, false, page, pageSize, "category_attribute_rules", cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveCategory([Bind(Prefix = "CategoryForm")] CategoryUpsertForm form)
        => base.SaveCategory(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> MoveCategory([Bind(Prefix = "MoveCategoryForm")] MoveCategoryForm form)
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
    public new async Task<IActionResult> CreateCategoryAttribute([Bind(Prefix = "AttributeForm")] AttributeDefinitionForm form)
        => KeepCategorySelectionOn(await base.CreateCategoryAttribute(form), nameof(Attributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> UpdateAttributeDefinition([Bind(Prefix = "AttributeUpdateForm")] AttributeDefinitionUpdateForm form)
        => KeepCategorySelectionOn(await base.UpdateAttributeDefinition(form), nameof(Attributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> ActivateAttributeDefinition(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.ActivateAttributeDefinition(categoryId, attributeId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> DeactivateAttributeDefinition(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.DeactivateAttributeDefinition(categoryId, attributeId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> DeleteAttributeDefinition(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.DeleteAttributeDefinition(categoryId, attributeId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> AddAttributeOption([Bind(Prefix = "OptionForm")] AttributeOptionForm form)
        => KeepCategorySelectionOn(await base.AddAttributeOption(form), nameof(Attributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> UpdateAttributeOption([Bind(Prefix = "OptionUpdateForm")] AttributeOptionUpdateForm form)
        => KeepCategorySelectionOn(await base.UpdateAttributeOption(form), nameof(Attributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> ActivateAttributeOption(string categoryId, string attributeId, string optionId)
        => KeepCategorySelectionOn(await base.ActivateAttributeOption(categoryId, attributeId, optionId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> DeactivateAttributeOption(string categoryId, string attributeId, string optionId)
        => KeepCategorySelectionOn(await base.DeactivateAttributeOption(categoryId, attributeId, optionId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> DeleteAttributeOption(string categoryId, string attributeId, string optionId)
        => KeepCategorySelectionOn(await base.DeleteAttributeOption(categoryId, attributeId, optionId), nameof(Attributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> AssignAttributeToCategory([Bind(Prefix = "AssignForm")] AssignAttributeForm form)
        => KeepCategorySelectionOn(await base.AssignAttributeToCategory(form), nameof(CategoryAttributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> UpdateCategoryAttributeRule([Bind(Prefix = "RuleForm")] CategoryAttributeRuleForm form)
        => KeepCategorySelectionOn(await base.UpdateCategoryAttributeRule(form), nameof(CategoryAttributes), form.CategoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> ActivateCategoryAttributeRule(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.ActivateCategoryAttributeRule(categoryId, attributeId), nameof(CategoryAttributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> DeactivateCategoryAttributeRule(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.DeactivateCategoryAttributeRule(categoryId, attributeId), nameof(CategoryAttributes), categoryId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new async Task<IActionResult> RemoveCategoryAttributeRule(string categoryId, string attributeId)
        => KeepCategorySelectionOn(await base.RemoveCategoryAttributeRule(categoryId, attributeId), nameof(CategoryAttributes), categoryId);

    private IActionResult KeepCategorySelectionOn(IActionResult result, string actionName, string? categoryId)
    {
        if (result is RedirectToActionResult redirect &&
            string.Equals(redirect.ActionName, nameof(Categories), StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(actionName, new { categoryId });
        }

        return result;
    }
}
