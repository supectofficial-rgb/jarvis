using Insurance.InventoryDashboard.Panel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public abstract partial class CatalogManagementController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> MoveCategory(MoveCategoryForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Move", "Category.Move"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه اهمهتهمهاهمه دستهمه?Oبهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!string.IsNullOrWhiteSpace(form.ParentCategoryId) &&
            string.Equals(form.CategoryId, form.ParentCategoryId, StringComparison.OrdinalIgnoreCase))
        {
            TempData["CatalogError"] = "دستهمه?Oبهمهدهمه همه.همه?Oتهمهاهمهد همهاهمهد خهمهدش باشد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();
        if (CreatesCategoryCycle(flatCategories, form.CategoryId, form.ParentCategoryId))
        {
            TempData["CatalogError"] = "اهمهتخاب همهاهمهد جدهمهد باعث ایجاد همهرخهمه در درخت دستهمه?Oبهمهدهمه همهOهمهOشهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var result = await _apiService.MoveCategoryAsync(form.CategoryId, form.ParentCategoryId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "دسته‌بندی با موفقیت منتقل شد." : result.ErrorMessage ?? "انتقال دسته‌بندی انجام نشد.";
        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateCategory(string categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Activate", "Category.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه دستهمه?Oبهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.ActivateCategoryAsync(categoryId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "دستهمه?Oبهمهدهمه فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه دستهمه?Oبهمهدهمه اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateCategory(string categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Deactivate", "Category.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه دستهمه?Oبهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeactivateCategoryAsync(categoryId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "دستهمه?Oبهمهدهمه غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه دستهمه?Oبهمهدهمه اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeleteCategory(string categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Delete", "Category.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف دستهمه?Oبهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeleteCategoryAsync(categoryId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "دستهمه?Oبهمهدهمه حذف شد." : result.ErrorMessage ?? "حذف دستهمه?Oبهمهدهمه اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ApplyCategoryBulkAction(
        string selectedIds,
        string bulkAction,
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var ids = ParseSelectedIds(selectedIds);
        if (ids.Count == 0)
        {
            TempData["CatalogError"] = "هیچ دسته‌بندی برای عملیات گروهی انتخاب نشده است.";
            return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
        }

        Func<string, string, Task<ApiResponse<bool>>> executor;
        string successMessage;
        string permissionError;

        if (string.Equals(bulkAction, "activate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Category.Activate", "Category.Activate"))
            {
                TempData["CatalogError"] = "دسترسی فعال‌سازی گروهی دسته‌بندی را ندارید.";
                return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.ActivateCategoryAsync(id, auth);
            successMessage = "فعال‌سازی گروهی دسته‌بندی انجام شد.";
            permissionError = "فعال‌سازی";
        }
        else if (string.Equals(bulkAction, "deactivate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Category.Deactivate", "Category.Deactivate"))
            {
                TempData["CatalogError"] = "دسترسی غیرفعال‌سازی گروهی دسته‌بندی را ندارید.";
                return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeactivateCategoryAsync(id, auth);
            successMessage = "غیرفعال‌سازی گروهی دسته‌بندی انجام شد.";
            permissionError = "غیرفعال‌سازی";
        }
        else if (string.Equals(bulkAction, "delete", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Category.Delete", "Category.Delete"))
            {
                TempData["CatalogError"] = "دسترسی حذف گروهی دسته‌بندی را ندارید.";
                return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeleteCategoryAsync(id, auth);
            successMessage = "حذف گروهی دسته‌بندی انجام شد.";
            permissionError = "حذف";
        }
        else
        {
            TempData["CatalogError"] = "نوع عملیات گروهی دسته‌بندی معتبر نیست.";
            return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
        }

        var successCount = 0;
        var errors = new List<string>();
        foreach (var id in ids)
        {
            var result = await executor(id, token);
            if (result.IsSuccess)
            {
                successCount++;
                continue;
            }

            errors.Add($"شناسه {id}: {result.ErrorMessage ?? $"خطا در {permissionError}"}");
        }

        if (successCount > 0)
        {
            TempData["CatalogSuccess"] = $"{successMessage} ({successCount} مورد)";
        }

        if (errors.Count > 0)
        {
            TempData["CatalogError"] = string.Join(" | ", errors.Take(3));
        }

        return RedirectToAction(nameof(Categories), new { categoryId, searchTerm, statusFilter, sort, page, pageSize });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> UpdateCategoryAttributeRule(CategoryAttributeRuleForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Rule.Update", "CategoryAttributeRule.Update"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه ویرایش همه^اهمهOهمه دستهمه?Oبهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var request = new UpdateCategoryAttributeRuleRequest
        {
            IsRequired = form.IsRequired,
            IsVariant = form.IsVariant,
            DisplayOrder = form.DisplayOrder,
            IsOverridden = form.IsOverridden,
            IsActive = form.IsActive
        };

        var result = await _apiService.UpdateCategoryAttributeRuleAsync(form.CategoryId, form.AttributeId, request, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "همهاهمه^همه اتریبیوت بهمه?OرهمهزرساهمهO شد." : result.ErrorMessage ?? "بهمه?OرهمهزرساهمهO همهاهمه^همه اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateCategoryAttributeRule(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Rule.Activate", "CategoryAttributeRule.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه rule را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.ActivateCategoryAttributeRuleAsync(categoryId, attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Rule فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه rule اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateCategoryAttributeRule(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Rule.Deactivate", "CategoryAttributeRule.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه rule را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeactivateCategoryAttributeRuleAsync(categoryId, attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Rule غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه rule اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> RemoveCategoryAttributeRule(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Rule.Remove", "CategoryAttributeRule.Remove", "CategoryAttributeRule.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف rule را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.RemoveCategoryAttributeRuleAsync(categoryId, attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Rule حذف شد." : result.ErrorMessage ?? "حذف rule اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> UpdateAttributeDefinition(AttributeDefinitionUpdateForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeDefinition.Update", "AttributeDefinition.Update"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه ویرایش اتریبیوت را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!AllowedAttributeDataTypes.Contains(form.DataType))
        {
            TempData["CatalogError"] = "نوع داده اتریبیوت نامعتبر است.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!AllowedAttributeScopes.Contains(form.Scope))
        {
            TempData["CatalogError"] = "سطح اعمال اتریبیوت نامعتبر است.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var request = new UpdateAttributeDefinitionRequest
        {
            Code = form.Code.Trim(),
            Name = form.Name.Trim(),
            DataType = form.DataType.Trim(),
            Scope = form.Scope.Trim(),
            DisplayOrder = form.DisplayOrder,
            IsActive = form.IsActive
        };

        var result = await _apiService.UpdateAttributeDefinitionAsync(form.AttributeId, request, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "بروزرسانی ویژگی انجام نشد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        // در تب ویژگی، کاربر اولویت/الزامی/واریانت‌ساز را هم مدیریت می‌کند؛
        // بنابراین اگر rule محلی وجود داشته باشد همان را هم به‌روزرسانی می‌کنیم.
        var canUpdateRule = IsAuthorizedFor(token, "Catalog.Category.Rule.Update", "CategoryAttributeRule.Update");
        if (canUpdateRule)
        {
            var rulesResult = await _apiService.GetCategoryAttributeRulesAsync(
                form.CategoryId,
                token,
                includeInherited: false,
                includeInactive: true);

            var localRule = (rulesResult.Data ?? new List<CategoryAttributeRuleModel>())
                .FirstOrDefault(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));

            if (localRule is not null)
            {
                var ruleUpdateResult = await _apiService.UpdateCategoryAttributeRuleAsync(
                    form.CategoryId,
                    form.AttributeId,
                    new UpdateCategoryAttributeRuleRequest
                    {
                        IsRequired = form.IsRequired,
                        IsVariant = form.IsVariant,
                        DisplayOrder = form.DisplayOrder,
                        IsOverridden = localRule.RuleIsOverridden,
                        IsActive = localRule.RuleIsActive
                    },
                    token);

                if (!ruleUpdateResult.IsSuccess)
                {
                    TempData["CatalogError"] = ruleUpdateResult.ErrorMessage ?? "بروزرسانی قانون ویژگی انجام نشد.";
                    return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
                }
            }
        }

        TempData["CatalogSuccess"] = "ویژگی با موفقیت بروزرسانی شد.";
        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateAttributeDefinition(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeDefinition.Activate", "AttributeDefinition.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه اتریبیوت را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.ActivateAttributeDefinitionAsync(attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "اتریبیوت فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه اتریبیوت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateAttributeDefinition(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeDefinition.Deactivate", "AttributeDefinition.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه اتریبیوت را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeactivateAttributeDefinitionAsync(attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "اتریبیوت غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه اتریبیوت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeleteAttributeDefinition(string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeDefinition.Delete", "AttributeDefinition.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف اتریبیوت را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeleteAttributeDefinitionAsync(attributeId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "اتریبیوت حذف شد." : result.ErrorMessage ?? "حذف اتریبیوت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> UpdateAttributeOption(AttributeOptionUpdateForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Update", "AttributeOption.Update"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه ویرایش option را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var request = new UpdateAttributeOptionRequest
        {
            OptionName = form.OptionName.Trim(),
            OptionValue = form.OptionValue.Trim(),
            DisplayOrder = form.DisplayOrder
        };
        var result = await _apiService.UpdateAttributeOptionAsync(form.AttributeId, form.OptionId, request, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Option بهمه?OرهمهزرساهمهO شد." : result.ErrorMessage ?? "بهمه?OرهمهزرساهمهO option اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateAttributeOption(string categoryId, string attributeId, string optionId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Activate", "AttributeOption.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه option را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.ActivateAttributeOptionAsync(attributeId, optionId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Option فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه option اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateAttributeOption(string categoryId, string attributeId, string optionId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Deactivate", "AttributeOption.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه option را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeactivateAttributeOptionAsync(attributeId, optionId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Option غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه option اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeleteAttributeOption(string categoryId, string attributeId, string optionId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Delete", "AttributeOption.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف option را همهدارهمهد.";
            return RedirectToAction(nameof(Categories), new { categoryId });
        }

        var result = await _apiService.DeleteAttributeOptionAsync(attributeId, optionId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Option حذف شد." : result.ErrorMessage ?? "حذف option اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Categories), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateProduct(string productId, string? categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Product.Activate", "Product.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه محصول را همهدارهمهد.";
            return RedirectToAction(nameof(Products), new { productId, categoryId });
        }

        var result = await _apiService.ActivateProductAsync(productId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "محصول فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه محصول اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Products), new { productId, categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateProduct(string productId, string? categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Product.Deactivate", "Product.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه محصول را همهدارهمهد.";
            return RedirectToAction(nameof(Products), new { productId, categoryId });
        }

        var result = await _apiService.DeactivateProductAsync(productId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "محصول غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه محصول اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Products), new { productId, categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeleteProduct(string productId, string? categoryId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Product.Delete", "Product.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف محصول را همهدارهمهد.";
            return RedirectToAction(nameof(Products), new { productId, categoryId });
        }

        var result = await _apiService.DeleteProductAsync(productId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "محصول حذف شد." : result.ErrorMessage ?? "حذف محصول اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Products), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ApplyProductBulkAction(
        string selectedIds,
        string bulkAction,
        string? categoryId,
        string? searchTerm,
        string? categoryFilterId,
        string? statusFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var ids = ParseSelectedIds(selectedIds);
        if (ids.Count == 0)
        {
            TempData["CatalogError"] = "هیچ محصولی برای عملیات گروهی انتخاب نشده است.";
            return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
        }

        Func<string, string, Task<ApiResponse<bool>>> executor;
        string successMessage;
        string operationName;

        if (string.Equals(bulkAction, "activate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Product.Activate", "Product.Activate"))
            {
                TempData["CatalogError"] = "دسترسی فعال‌سازی گروهی محصول را ندارید.";
                return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.ActivateProductAsync(id, auth);
            successMessage = "فعال‌سازی گروهی محصول انجام شد.";
            operationName = "فعال‌سازی";
        }
        else if (string.Equals(bulkAction, "deactivate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Product.Deactivate", "Product.Deactivate"))
            {
                TempData["CatalogError"] = "دسترسی غیرفعال‌سازی گروهی محصول را ندارید.";
                return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeactivateProductAsync(id, auth);
            successMessage = "غیرفعال‌سازی گروهی محصول انجام شد.";
            operationName = "غیرفعال‌سازی";
        }
        else if (string.Equals(bulkAction, "delete", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Product.Delete", "Product.Delete"))
            {
                TempData["CatalogError"] = "دسترسی حذف گروهی محصول را ندارید.";
                return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeleteProductAsync(id, auth);
            successMessage = "حذف گروهی محصول انجام شد.";
            operationName = "حذف";
        }
        else
        {
            TempData["CatalogError"] = "نوع عملیات گروهی محصول معتبر نیست.";
            return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
        }

        var successCount = 0;
        var errors = new List<string>();
        foreach (var id in ids)
        {
            var result = await executor(id, token);
            if (result.IsSuccess)
            {
                successCount++;
                continue;
            }

            errors.Add($"شناسه {id}: {result.ErrorMessage ?? $"خطا در {operationName}"}");
        }

        if (successCount > 0)
        {
            TempData["CatalogSuccess"] = $"{successMessage} ({successCount} مورد)";
        }

        if (errors.Count > 0)
        {
            TempData["CatalogError"] = string.Join(" | ", errors.Take(3));
        }

        return RedirectToAction(nameof(Products), new { categoryId, searchTerm, categoryFilterId, statusFilter, sort, page, pageSize });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ChangeProductCategory(ProductCategoryChangeForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Product.ChangeCategory", "Product.ChangeCategory"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه تغهمهOر دستهمه?Oبهمهدهمه محصول را همهدارهمهد.";
            return RedirectToAction(nameof(Products), new { productId = form.ProductId, categoryId = form.CurrentCategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Products), new { productId = form.ProductId, categoryId = form.CurrentCategoryId });
        }

        var request = new ChangeProductCategoryRequest
        {
            CategoryId = form.NewCategoryId
        };

        var result = await _apiService.ChangeProductCategoryAsync(form.ProductId, request, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "دستهمه?Oبهمهدهمه محصول تغهمهOر کرد." : result.ErrorMessage ?? "تغهمهOر دستهمه?Oبهمهدهمه محصول اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Products), new { productId = form.ProductId, categoryId = form.NewCategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ActivateVariant(string productId, string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.Activate", "ProductVariant.Activate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه فعالهمهOسازهمه همهارهمهاهمهت را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.ActivateProductVariantAsync(variantId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "همهارهمهاهمهت فعال شد." : result.ErrorMessage ?? "فعالهمهOسازهمه همهارهمهاهمهت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeactivateVariant(string productId, string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.Deactivate", "ProductVariant.Deactivate"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه غیرفعالهمهOسازهمه همهارهمهاهمهت را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.DeactivateProductVariantAsync(variantId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "همهارهمهاهمهت غیرفعال شد." : result.ErrorMessage ?? "غیرفعالهمهOسازهمه همهارهمهاهمهت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> DeleteVariant(string productId, string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.Delete", "ProductVariant.Delete"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف همهارهمهاهمهت را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.DeleteProductVariantAsync(variantId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "همهارهمهاهمهت حذف شد." : result.ErrorMessage ?? "حذف همهارهمهاهمهت اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ApplyVariantBulkAction(
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
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var ids = ParseSelectedIds(selectedIds);
        if (ids.Count == 0)
        {
            TempData["CatalogError"] = "هیچ واریانتی برای عملیات گروهی انتخاب نشده است.";
            return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
        }

        Func<string, string, Task<ApiResponse<bool>>> executor;
        string successMessage;
        string operationName;

        if (string.Equals(bulkAction, "activate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Variant.Activate", "ProductVariant.Activate"))
            {
                TempData["CatalogError"] = "دسترسی فعال‌سازی گروهی واریانت را ندارید.";
                return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.ActivateProductVariantAsync(id, auth);
            successMessage = "فعال‌سازی گروهی واریانت انجام شد.";
            operationName = "فعال‌سازی";
        }
        else if (string.Equals(bulkAction, "deactivate", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Variant.Deactivate", "ProductVariant.Deactivate"))
            {
                TempData["CatalogError"] = "دسترسی غیرفعال‌سازی گروهی واریانت را ندارید.";
                return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeactivateProductVariantAsync(id, auth);
            successMessage = "غیرفعال‌سازی گروهی واریانت انجام شد.";
            operationName = "غیرفعال‌سازی";
        }
        else if (string.Equals(bulkAction, "delete", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsAuthorizedFor(token, "Catalog.Variant.Delete", "ProductVariant.Delete"))
            {
                TempData["CatalogError"] = "دسترسی حذف گروهی واریانت را ندارید.";
                return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
            }

            executor = (id, auth) => _apiService.DeleteProductVariantAsync(id, auth);
            successMessage = "حذف گروهی واریانت انجام شد.";
            operationName = "حذف";
        }
        else
        {
            TempData["CatalogError"] = "نوع عملیات گروهی واریانت معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
        }

        var successCount = 0;
        var errors = new List<string>();
        foreach (var id in ids)
        {
            var result = await executor(id, token);
            if (result.IsSuccess)
            {
                successCount++;
                continue;
            }

            errors.Add($"شناسه {id}: {result.ErrorMessage ?? $"خطا در {operationName}"}");
        }

        if (successCount > 0)
        {
            TempData["CatalogSuccess"] = $"{successMessage} ({successCount} مورد)";
        }

        if (errors.Count > 0)
        {
            TempData["CatalogError"] = string.Join(" | ", errors.Take(3));
        }

        return RedirectToAction(nameof(Variants), new { productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ChangeVariantTrackingPolicy(string productId, string variantId, string trackingPolicy)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.ChangeTrackingPolicy", "ProductVariant.ChangeTrackingPolicy"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه تغهمهOر tracking policy را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        if (await IsVariantInventoryMovementLockedAsync(variantId, token))
        {
            TempData["CatalogError"] = "این واریانت قفل حرکت موجودی شده است و تغییر Tracking Policy برای آن مجاز نیست.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        if (!AllowedTrackingPolicies.Contains(trackingPolicy))
        {
            TempData["CatalogError"] = "Tracking policy همهعتبر همهOست.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.ChangeProductVariantTrackingPolicyAsync(
            variantId,
            new ChangeVariantTrackingPolicyRequest { TrackingPolicy = trackingPolicy },
            token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Tracking policy همهارهمهاهمهت تغهمهOر کرد." : result.ErrorMessage ?? "تغهمهOر tracking policy اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> ChangeVariantBaseUom(string productId, string variantId, string baseUomRef)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.ChangeBaseUom", "ProductVariant.ChangeBaseUom"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه تغهمهOر همهاحد پاهمه? همهارهمهاهمهت را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        if (await IsVariantInventoryMovementLockedAsync(variantId, token))
        {
            TempData["CatalogError"] = "این واریانت قفل حرکت موجودی شده است و تغییر Base UOM برای آن مجاز نیست.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var validUom = (uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>())
            .Any(x => string.Equals(x.Id, baseUomRef, StringComparison.OrdinalIgnoreCase));
        if (!validUom)
        {
            TempData["CatalogError"] = "همهاحد اهمهتخابهمهOشدهمه همهعتبر همهOست.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.ChangeProductVariantBaseUomAsync(
            variantId,
            new ChangeVariantBaseUomRequest { BaseUomRef = baseUomRef },
            token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "همهاحد پاهمه? همهارهمهاهمهت تغهمهOر کرد." : result.ErrorMessage ?? "تغهمهOر همهاحد پاهمه? اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> LockVariantInventoryMovement(string productId, string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.LockInventoryMovement", "ProductVariant.LockInventoryMovement"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه همهفهمه?Oکردهمه حرکت همه^جهمهدهمه را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        if (await IsVariantInventoryMovementLockedAsync(variantId, token))
        {
            TempData["CatalogNotice"] = "حرکت موجودی این واریانت قبلا قفل شده است.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.LockProductVariantInventoryMovementAsync(variantId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "حرکت همه^جهمهدهمه همهارهمهاهمهت همهفهمه شد." : result.ErrorMessage ?? "همهفهمه حرکت همه^جهمهدهمه اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> UpsertVariantUomConversion(VariantUomConversionForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.VariantUomConversion.Upsert", "ProductVariantUomConversion.Upsert"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه ثبت conversion را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (string.Equals(form.FromUomRef, form.ToUomRef, StringComparison.OrdinalIgnoreCase))
        {
            TempData["CatalogError"] = "From UOM همه To UOM همهباهمهد همهکساهمه باشهمهد.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var request = new UpsertVariantUomConversionRequest
        {
            FromUomRef = form.FromUomRef,
            ToUomRef = form.ToUomRef,
            Factor = form.Factor,
            RoundingMode = form.RoundingMode,
            IsBasePath = form.IsBasePath
        };

        var result = await _apiService.UpsertVariantUomConversionAsync(form.VariantId, request, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Conversion ذخهمهرهمه شد." : result.ErrorMessage ?? "ذخهمهرهمه conversion اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> RemoveVariantUomConversion(string productId, string variantId, string fromUomRef, string toUomRef)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.VariantUomConversion.Remove", "ProductVariantUomConversion.Remove"))
        {
            TempData["CatalogError"] = "شهمها دسترسهمه حذف conversion را همهدارهمهد.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.RemoveVariantUomConversionAsync(variantId, fromUomRef, toUomRef, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "Conversion حذف شد." : result.ErrorMessage ?? "حذف conversion اهمهجاهمه همهشد.";
        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    private async Task<bool> IsVariantInventoryMovementLockedAsync(string? variantId, string token)
    {
        if (string.IsNullOrWhiteSpace(variantId))
        {
            return false;
        }

        var detailsResult = await _apiService.GetProductVariantFullDetailsAsync(variantId, token);
        return detailsResult.Data?.InventoryMovementLocked ?? false;
    }

    private static IReadOnlyList<string> ParseSelectedIds(string? selectedIds)
    {
        if (string.IsNullOrWhiteSpace(selectedIds))
        {
            return Array.Empty<string>();
        }

        return selectedIds
            .Split(new[] { ',', ';', '\n', '\r', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    protected bool IsAuthorizedFor(string token, params string[] aliases)
    {
        var roles = ResolveRolesFromSession(token);
        if (roles.Any(x =>
            string.Equals(x, "SysAdmin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x, "Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var permissions = ResolvePermissionsFromSession();
        if (permissions.Count == 0)
        {
            return true;
        }

        if (permissions.Any(x => string.Equals(x, "*", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return aliases.Any(alias => permissions.Any(permission => string.Equals(permission, alias, StringComparison.OrdinalIgnoreCase)));
    }
}



