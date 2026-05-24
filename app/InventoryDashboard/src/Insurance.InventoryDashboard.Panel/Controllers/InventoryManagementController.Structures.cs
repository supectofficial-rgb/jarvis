using System;
using System.Collections.Generic;
using System.Linq;
using Insurance.InventoryDashboard.Panel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class InventoryManagementController
{
    [HttpGet]
    public Task<IActionResult> WarehouseStructures(
        string? warehouseId,
        string? structureId,
        string? mode,
        string? tab,
        CancellationToken cancellationToken = default)
        => BuildWarehouseStructuresPageAsync(warehouseId, structureId, mode, tab, cancellationToken);

    [HttpGet]
    public async Task<IActionResult> WarehouseStructureTree(string warehouseId, bool includeInactive = true, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربر در دسترس نیست." });
        }

        if (!IsAuthorizedFor(token, "Inventory.Warehouse.View", "Warehouse.Read", "Warehouse.Search"))
        {
            return StatusCode(403, new { isSuccess = false, errorMessage = "شما دسترسی مشاهده ساختار انبار را ندارید." });
        }

        if (string.IsNullOrWhiteSpace(warehouseId))
        {
            return Json(new { isSuccess = true, items = Array.Empty<LocationStructureTreeItemModel>() });
        }

        var result = await _apiService.GetWarehouseLocationStructureTreeAsync(warehouseId.Trim(), token, includeInactive);
        return Json(new
        {
            isSuccess = result.IsSuccess,
            errorMessage = result.ErrorMessage,
            items = result.Data ?? new List<LocationStructureTreeItemModel>()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveWarehouseStructureNode([Bind(Prefix = "StructureNodeForm")] LocationStructureNodeForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(WarehouseStructures), new { warehouseId = form.WarehouseRef, structureId = form.LocationStructureBusinessKey, mode = Request.Query["mode"].ToString(), tab = "nodes" });
        }

        var result = string.IsNullOrWhiteSpace(form.LocationStructureBusinessKey)
            ? await _apiService.CreateLocationStructureNodeAsync(form, token)
            : await _apiService.UpdateLocationStructureNodeAsync(form.LocationStructureBusinessKey, form, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "ساختار لوکیشن با موفقیت ذخیره شد." : result.ErrorMessage ?? "ذخیره ساختار با خطا مواجه شد.";

        return RedirectToAction(nameof(WarehouseStructures), new { warehouseId = form.WarehouseRef, structureId = form.LocationStructureBusinessKey, mode = Request.Query["mode"].ToString(), tab = "nodes" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveWarehouseStructureValue([Bind(Prefix = "StructureValueForm")] LocationStructureValueForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        form.Code = form.Code?.Trim() ?? string.Empty;
        form.Name = form.Name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(form.Name) && !string.IsNullOrWhiteSpace(form.Code))
        {
            form.Name = form.Code;
        }

        if (string.IsNullOrWhiteSpace(form.Code) && !string.IsNullOrWhiteSpace(form.Name))
        {
            form.Code = form.Name;
        }

        if (!TryValidateModel(form))
        {
            var error = ExtractModelError(ModelState);
            TempData["CatalogError"] = error;
            if (IsAjaxRequest())
            {
                return Json(new { isSuccess = false, errorMessage = error });
            }

            return RedirectToAction(nameof(WarehouseStructures), new { warehouseId = Request.Query["warehouseId"].ToString(), structureId = form.StructureRef, mode = Request.Query["mode"].ToString(), tab = "nodes" });
        }

        if (string.IsNullOrWhiteSpace(form.LocationStructureValueBusinessKey))
        {
            var existingValuesResult = await _apiService.GetLocationStructureValuesAsync(form.StructureRef, token, includeInactive: true);
            var existingValue = existingValuesResult.Data?.FirstOrDefault(x => string.Equals(x.Code?.Trim(), form.Code, StringComparison.OrdinalIgnoreCase));
            if (existingValue is not null)
            {
                TempData["CatalogSuccess"] = "مقدار انتخاب‌شده از قبل وجود داشت و دوباره استفاده شد.";
                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        isSuccess = true,
                        isReused = true,
                        structureRef = form.StructureRef,
                        value = existingValue.Code,
                        name = existingValue.Name,
                        locationStructureValueBusinessKey = existingValue.LocationStructureValueBusinessKey
                    });
                }

                return RedirectToAction(nameof(WarehouseStructures), new { warehouseId = Request.Query["warehouseId"].ToString(), structureId = form.StructureRef, mode = Request.Query["mode"].ToString(), tab = "nodes" });
            }
        }

        var result = string.IsNullOrWhiteSpace(form.LocationStructureValueBusinessKey)
            ? await _apiService.CreateLocationStructureValueAsync(form, token)
            : await _apiService.UpdateLocationStructureValueAsync(form.LocationStructureValueBusinessKey, form, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "مقدار ساختار با موفقیت ذخیره شد." : result.ErrorMessage ?? "ذخیره مقدار ساختار با خطا مواجه شد.";

        if (IsAjaxRequest())
        {
            return Json(new
            {
                isSuccess = result.IsSuccess,
                errorMessage = result.ErrorMessage,
                structureRef = form.StructureRef,
                value = form.Code,
                name = form.Name,
                locationStructureValueBusinessKey = form.LocationStructureValueBusinessKey
            });
        }

        return RedirectToAction(nameof(WarehouseStructures), new { warehouseId = Request.Query["warehouseId"].ToString(), structureId = form.StructureRef, mode = Request.Query["mode"].ToString(), tab = "nodes" });
    }

    private async Task<IActionResult> BuildWarehouseStructuresPageAsync(
        string? warehouseId,
        string? structureId,
        string? mode,
        string? tab,
        CancellationToken cancellationToken)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Warehouse.View", "Warehouse.Read", "Warehouse.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده مدیریت ساختار انبار را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }

        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "inventory_management", "warehouses");

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var warehouseLookup = warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>();
        var selectedWarehouseId = string.IsNullOrWhiteSpace(warehouseId) ? warehouseLookup.FirstOrDefault()?.WarehouseBusinessKey : warehouseId.Trim();

        var structuresResult = !string.IsNullOrWhiteSpace(selectedWarehouseId)
            ? await _apiService.GetWarehouseLocationStructureTreeAsync(selectedWarehouseId, token, includeInactive: true)
            : new ApiResponse<List<LocationStructureTreeItemModel>> { IsSuccess = true, Data = new List<LocationStructureTreeItemModel>() };

        var model = new WarehouseStructurePageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            WarehouseLookup = warehouseLookup,
            SelectedWarehouseId = selectedWarehouseId,
            SelectedStructureId = structureId,
            StructureMode = string.Equals(mode, "select", StringComparison.OrdinalIgnoreCase) ? "select" : "manage",
            StructureAjaxTarget = string.Equals(mode, "select", StringComparison.OrdinalIgnoreCase) ? "#warehouse-structure-panel" : "#warehouse-structure-content",
            Structures = structuresResult.Data ?? new List<LocationStructureTreeItemModel>(),
            ErrorMessage = JoinErrors(warehouseLookupResult.ErrorMessage, structuresResult.ErrorMessage),
            StructureNodeForm = new LocationStructureNodeForm
            {
                WarehouseRef = selectedWarehouseId ?? string.Empty,
                LocationStructureBusinessKey = structureId
            },
            StructureValueForm = new LocationStructureValueForm
            {
                StructureRef = structureId ?? string.Empty,
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/InventoryManagement/WarehouseStructures.cshtml", model);
    }
}
