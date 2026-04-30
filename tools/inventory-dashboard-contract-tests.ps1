param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

$panel = Join-Path $Root 'app\InventoryDashboard\src\Insurance.InventoryDashboard.Panel'
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure([string]$message) {
    $failures.Add($message) | Out-Null
}

function Read-Text([string]$relativePath) {
    $path = Join-Path $panel $relativePath
    if (-not (Test-Path -LiteralPath $path)) {
        Add-Failure "Missing file: $relativePath"
        return ''
    }

    return Get-Content -LiteralPath $path -Raw
}

function Assert-Contains([string]$text, [string]$needle, [string]$message) {
    if (-not $text.Contains($needle)) {
        Add-Failure $message
    }
}

function Assert-NotContains([string]$text, [string]$needle, [string]$message) {
    if ($text.Contains($needle)) {
        Add-Failure $message
    }
}

$menuJson = Read-Text 'dashboard-widgets.json'
$catalogController = Read-Text 'Controllers\CatalogManagementController.cs'
$categoryController = Read-Text 'Controllers\CategoryManagementController.cs'
$productController = Read-Text 'Controllers\ProductManagementController.cs'
$variantController = Read-Text 'Controllers\VariantManagementController.cs'
$categoryView = Read-Text 'Views\CatalogManagement\Categories.cshtml'
$productView = Read-Text 'Views\CatalogManagement\Products.cshtml'
$variantView = Read-Text 'Views\CatalogManagement\Variants.cshtml'

Assert-NotContains $menuJson '#attribute-tab' 'Menu must not link category attributes by hash.'
Assert-NotContains $menuJson '#rule-tab' 'Menu must not link category rules by hash.'
Assert-NotContains $menuJson '?item=price_' 'Menu must not link pricing pages by query-string tabs.'
Assert-NotContains $menuJson 'activeItem=locations' 'Menu must not link inventory locations by tab query.'

$expectedRoutes = @(
    '/CategoryManagement/Categories',
    '/CategoryManagement/Attributes',
    '/CategoryManagement/CategoryAttributes',
    '/ProductManagement/Products',
    '/VariantManagement/Variants',
    '/InventoryManagement/Warehouses',
    '/InventoryManagement/Locations',
    '/PricingManagement/PriceTypes',
    '/PricingManagement/PriceChannels',
    '/PricingManagement/VariantPrices',
    '/InvoiceManagement/Proformas',
    '/InvoiceManagement/Invoices'
)

foreach ($route in $expectedRoutes) {
    Assert-Contains $menuJson $route "Missing menu route: $route"
}

$categoryBindings = @{
    'SaveCategory' = 'CategoryForm'
    'MoveCategory' = 'MoveCategoryForm'
    'CreateCategoryAttribute' = 'AttributeForm'
    'UpdateAttributeDefinition' = 'AttributeUpdateForm'
    'AddAttributeOption' = 'OptionForm'
    'UpdateAttributeOption' = 'OptionUpdateForm'
    'AssignAttributeToCategory' = 'AssignForm'
    'UpdateCategoryAttributeRule' = 'RuleForm'
}

foreach ($binding in $categoryBindings.GetEnumerator()) {
    Assert-Contains $categoryController "[Bind(Prefix = `"$($binding.Value)`")]" "Category action $($binding.Key) must bind prefix $($binding.Value)."
    Assert-Contains $categoryView "asp-for=`"$($binding.Value)." "Category view must post fields with prefix $($binding.Value)."
}

$productBindings = @{
    'SaveProduct' = 'ProductForm'
    'ChangeProductCategory' = 'ProductCategoryChangeForm'
    'SetProductAttributeValue' = 'ProductAttributeForm'
}

foreach ($binding in $productBindings.GetEnumerator()) {
    Assert-Contains $productController "[Bind(Prefix = `"$($binding.Value)`")]" "Product action $($binding.Key) must bind prefix $($binding.Value)."
    Assert-Contains $productView "asp-for=`"$($binding.Value)." "Product view must post fields with prefix $($binding.Value)."
}

$variantBindings = @{
    'SaveVariant' = 'VariantForm'
    'SetVariantAttributeValue' = 'VariantAttributeForm'
    'UpsertVariantUomConversion' = 'VariantUomConversionForm'
}

foreach ($binding in $variantBindings.GetEnumerator()) {
    Assert-Contains $variantController "[Bind(Prefix = `"$($binding.Value)`")]" "Variant action $($binding.Key) must bind prefix $($binding.Value)."
    Assert-Contains $variantView "asp-for=`"$($binding.Value)." "Variant view must post fields with prefix $($binding.Value)."
}

Assert-Contains $categoryView 'name="RuleForm.CategoryId"' 'Rule edit modal must post RuleForm.CategoryId.'
Assert-Contains $categoryView 'name="RuleForm.AttributeId"' 'Rule edit modal must post RuleForm.AttributeId.'
Assert-Contains $categoryView 'name="RuleForm.DisplayOrder"' 'Rule edit modal must post RuleForm.DisplayOrder.'
Assert-Contains $categoryView 'data-rule-display-order="@rule.RuleDisplayOrder"' 'Rule edit select options must carry rule display order for client-side sync.'
Assert-Contains $categoryView 'function applyRuleOptionToEditForm()' 'Rule edit form must sync fields when the selected rule changes.'
Assert-Contains $categoryView 'localRules.Count > 0' 'Rule edit submit button must be disabled when no local rules exist.'
Assert-Contains $categoryView 'asp-route-createNew="true"' 'Category create-new link must force create mode instead of auto-selecting the first category.'
Assert-Contains $categoryController 'bool createNew = false' 'Category page action must accept createNew mode.'
Assert-Contains $categoryController '[Bind(Prefix = "CategoryForm")]' 'SaveCategory must bind CategoryForm.* fields.'
Assert-Contains $categoryView 'string.Equals(x, "SysAdmin", StringComparison.OrdinalIgnoreCase)' 'Category view permissions must treat SysAdmin like an admin.'
Assert-Contains $categoryController 'ActivateAttributeDefinition' 'Category controller must expose ActivateAttributeDefinition used by the attributes page.'
Assert-Contains $categoryController 'DeactivateAttributeDefinition' 'Category controller must expose DeactivateAttributeDefinition used by the attributes page.'
Assert-Contains $categoryController 'DeleteAttributeDefinition' 'Category controller must expose DeleteAttributeDefinition used by the attributes page.'
Assert-Contains $categoryView 'asp-for="AttributeUpdateForm.DataType" class="form-control js-search-select"' 'Attribute edit DataType must be a constrained select.'
Assert-Contains $categoryView 'asp-for="AttributeUpdateForm.Scope" class="form-control js-search-select"' 'Attribute edit Scope must be a constrained select.'
Assert-Contains $productView 'asp-route-createNew="true"' 'Product create-new link must force create mode instead of auto-selecting the first product.'
Assert-Contains $productController 'bool createNew = false' 'Product page action must accept createNew mode.'
Assert-Contains $catalogController 'var isProductCreateMode = createNew;' 'Product page must derive a real create mode from createNew.'
Assert-Contains $catalogController 'var selectedProductId = isProductCreateMode' 'Product create mode must control selected product resolution.'
Assert-Contains $productView 'string.Equals(x, "SysAdmin", StringComparison.OrdinalIgnoreCase)' 'Product view permissions must treat SysAdmin like an admin.'
Assert-Contains $variantView 'asp-route-createNew="true"' 'Variant create-new link must force create mode instead of auto-selecting the first variant.'
Assert-Contains $variantController 'bool createNew = false' 'Variant page action must accept createNew mode.'
Assert-Contains $catalogController 'var isVariantCreateMode = createNew;' 'Variant page must derive a real create mode from createNew.'
Assert-Contains $catalogController 'var selectedVariantId = isVariantCreateMode' 'Variant create mode must control selected variant resolution.'
Assert-Contains $catalogController 'TempData["CatalogSuccess"] = string.IsNullOrWhiteSpace(form.VariantId)' 'SaveVariant must set success feedback so submitted drafts are cleared.'
Assert-Contains $variantView 'string.Equals(x, "SysAdmin", StringComparison.OrdinalIgnoreCase)' 'Variant view permissions must treat SysAdmin like an admin.'

if ($failures.Count -gt 0) {
    Write-Host 'Inventory dashboard contract tests failed:' -ForegroundColor Red
    foreach ($failure in $failures) {
        Write-Host " - $failure" -ForegroundColor Red
    }
    exit 1
}

Write-Host 'Inventory dashboard contract tests passed.' -ForegroundColor Green
