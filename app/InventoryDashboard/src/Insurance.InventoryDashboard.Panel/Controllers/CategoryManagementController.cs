using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class CategoryManagementController : CatalogManagementController
{
    public CategoryManagementController(
        ICatalogApiService apiService,
        IDashboardConfigService dashboardConfigService,
        ILogger<CatalogManagementController> logger)
        : base(apiService, dashboardConfigService, logger)
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

    [HttpGet]
    public async Task<IActionResult> GetAttributeOptions(string attributeId)
    {
        if (!TryGetToken(out var token))
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربری منقضی شده است." });

        if (string.IsNullOrWhiteSpace(attributeId))
            return BadRequest(new { isSuccess = false, errorMessage = "اتریبیوت انتخاب نشده است." });

        var result = await _apiService.GetAttributeOptionsByAttributeIdAsync(attributeId, token, onlyActive: false);
        return Json(new
        {
            isSuccess = result.IsSuccess,
            errorMessage = result.ErrorMessage,
            items = result.Data ?? new List<AttributeOptionModel>()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAttributeOptionAjax([Bind(Prefix = "OptionForm")] AttributeOptionForm form)
    {
        if (!TryGetToken(out var token))
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربری منقضی شده است." });

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Create", "AttributeOption.Create"))
            return StatusCode(StatusCodes.Status403Forbidden, new { isSuccess = false, errorMessage = "شما دسترسی افزودن آپشن را ندارید." });

        if (!TryValidateModel(form))
            return BadRequest(new { isSuccess = false, errorMessage = ExtractModelError(ModelState) });

        var result = await _apiService.AddAttributeOptionAsync(
            form.AttributeId,
            new AddAttributeOptionRequest
            {
                OptionName = form.OptionName.Trim(),
                OptionValue = form.OptionValue.Trim(),
                DisplayOrder = form.DisplayOrder
            },
            token);

        return await OptionsMutationResult(form.AttributeId, token, result, "آپشن با موفقیت اضافه شد.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAttributeOptionAjax([Bind(Prefix = "OptionUpdateForm")] AttributeOptionUpdateForm form)
    {
        if (!TryGetToken(out var token))
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربری منقضی شده است." });

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Update", "AttributeOption.Update"))
            return StatusCode(StatusCodes.Status403Forbidden, new { isSuccess = false, errorMessage = "شما دسترسی ویرایش آپشن را ندارید." });

        if (!TryValidateModel(form))
            return BadRequest(new { isSuccess = false, errorMessage = ExtractModelError(ModelState) });

        var result = await _apiService.UpdateAttributeOptionAsync(
            form.AttributeId,
            form.OptionId,
            new UpdateAttributeOptionRequest
            {
                OptionName = form.OptionName.Trim(),
                OptionValue = form.OptionValue.Trim(),
                DisplayOrder = form.DisplayOrder
            },
            token);

        return await OptionsMutationResult(form.AttributeId, token, result, "آپشن با موفقیت ویرایش شد.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttributeOptionAjax(string attributeId, string optionId)
    {
        if (!TryGetToken(out var token))
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربری منقضی شده است." });

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Delete", "AttributeOption.Delete"))
            return StatusCode(StatusCodes.Status403Forbidden, new { isSuccess = false, errorMessage = "شما دسترسی حذف آپشن را ندارید." });

        if (string.IsNullOrWhiteSpace(attributeId) || string.IsNullOrWhiteSpace(optionId))
            return BadRequest(new { isSuccess = false, errorMessage = "اتریبیوت یا آپشن انتخاب نشده است." });

        var result = await _apiService.DeleteAttributeOptionAsync(attributeId, optionId, token);
        if (!result.IsSuccess && IsNotFound(result.ErrorMessage))
        {
            return await OptionsMutationResult(
                attributeId,
                token,
                new ApiResponse<bool> { IsSuccess = true, Data = true },
                "آپشن قبلا حذف شده است.",
                optionId);
        }

        return await OptionsMutationResult(attributeId, token, result, "آپشن با موفقیت حذف شد.", optionId);
    }

    private async Task<IActionResult> OptionsMutationResult(
        string attributeId,
        string token,
        ApiResponse<bool> commandResult,
        string successMessage,
        string? removedOptionId = null)
    {
        if (!commandResult.IsSuccess)
            return BadRequest(new { isSuccess = false, errorMessage = commandResult.ErrorMessage ?? "عملیات با خطا مواجه شد." });

        var optionsResult = await _apiService.GetAttributeOptionsByAttributeIdAsync(attributeId, token, onlyActive: false);
        if (!optionsResult.IsSuccess)
            return BadRequest(new { isSuccess = false, errorMessage = optionsResult.ErrorMessage ?? "بارگذاری لیست آپشن‌ها انجام نشد." });

        var items = optionsResult.Data ?? new List<AttributeOptionModel>();
        if (!string.IsNullOrWhiteSpace(removedOptionId))
        {
            items = items
                .Where(x => !string.Equals(x.Id, removedOptionId, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(new
        {
            isSuccess = true,
            message = successMessage,
            items
        });
    }

    private static bool IsNotFound(string? message)
        => !string.IsNullOrWhiteSpace(message)
           && message.Contains("not found", StringComparison.OrdinalIgnoreCase);

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
