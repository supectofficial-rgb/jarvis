using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public sealed class CatalogApiService : ICatalogApiService
{
    private readonly IApiService _apiService;

    public CatalogApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<ApiResponse<bool>> CreateCategoryAsync(UpsertCategoryRequest request, string token) => _apiService.CreateCategoryAsync(request, token);
    public Task<ApiResponse<bool>> UpdateCategoryAsync(string categoryId, UpsertCategoryRequest request, string token) => _apiService.UpdateCategoryAsync(categoryId, request, token);
    public Task<ApiResponse<bool>> MoveCategoryAsync(string categoryId, string? parentCategoryId, string token) => _apiService.MoveCategoryAsync(categoryId, parentCategoryId, token);
    public Task<ApiResponse<bool>> ActivateCategoryAsync(string categoryId, string token) => _apiService.ActivateCategoryAsync(categoryId, token);
    public Task<ApiResponse<bool>> DeactivateCategoryAsync(string categoryId, string token) => _apiService.DeactivateCategoryAsync(categoryId, token);
    public Task<ApiResponse<bool>> DeleteCategoryAsync(string categoryId, string token) => _apiService.DeleteCategoryAsync(categoryId, token);
    public Task<ApiResponse<List<CategoryNodeModel>>> GetCategoryTreeAsync(string token) => _apiService.GetCategoryTreeAsync(token);
    public Task<ApiResponse<List<CategoryAttributeRuleModel>>> GetCategoryAttributeRulesAsync(string categoryId, string token, bool includeInherited = true, bool includeInactive = true) => _apiService.GetCategoryAttributeRulesAsync(categoryId, token, includeInherited, includeInactive);
    public Task<ApiResponse<bool>> AddCategoryAttributeRuleAsync(string categoryId, AddCategoryAttributeRuleRequest request, string token) => _apiService.AddCategoryAttributeRuleAsync(categoryId, request, token);
    public Task<ApiResponse<bool>> UpdateCategoryAttributeRuleAsync(string categoryId, string attributeId, UpdateCategoryAttributeRuleRequest request, string token) => _apiService.UpdateCategoryAttributeRuleAsync(categoryId, attributeId, request, token);
    public Task<ApiResponse<bool>> ActivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) => _apiService.ActivateCategoryAttributeRuleAsync(categoryId, attributeId, token);
    public Task<ApiResponse<bool>> DeactivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) => _apiService.DeactivateCategoryAttributeRuleAsync(categoryId, attributeId, token);
    public Task<ApiResponse<bool>> RemoveCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) => _apiService.RemoveCategoryAttributeRuleAsync(categoryId, attributeId, token);
    public Task<ApiResponse<bool>> CreateAttributeDefinitionAsync(string categoryId, CreateAttributeDefinitionRequest request, string token) => _apiService.CreateAttributeDefinitionAsync(categoryId, request, token);
    public Task<ApiResponse<bool>> UpdateAttributeDefinitionAsync(string attributeId, UpdateAttributeDefinitionRequest request, string token) => _apiService.UpdateAttributeDefinitionAsync(attributeId, request, token);
    public Task<ApiResponse<bool>> ActivateAttributeDefinitionAsync(string attributeId, string token) => _apiService.ActivateAttributeDefinitionAsync(attributeId, token);
    public Task<ApiResponse<bool>> DeactivateAttributeDefinitionAsync(string attributeId, string token) => _apiService.DeactivateAttributeDefinitionAsync(attributeId, token);
    public Task<ApiResponse<bool>> DeleteAttributeDefinitionAsync(string attributeId, string token) => _apiService.DeleteAttributeDefinitionAsync(attributeId, token);
    public Task<ApiResponse<bool>> AddAttributeOptionAsync(string attributeId, AddAttributeOptionRequest request, string token) => _apiService.AddAttributeOptionAsync(attributeId, request, token);
    public Task<ApiResponse<bool>> UpdateAttributeOptionAsync(string attributeId, string optionId, UpdateAttributeOptionRequest request, string token) => _apiService.UpdateAttributeOptionAsync(attributeId, optionId, request, token);
    public Task<ApiResponse<bool>> ActivateAttributeOptionAsync(string attributeId, string optionId, string token) => _apiService.ActivateAttributeOptionAsync(attributeId, optionId, token);
    public Task<ApiResponse<bool>> DeactivateAttributeOptionAsync(string attributeId, string optionId, string token) => _apiService.DeactivateAttributeOptionAsync(attributeId, optionId, token);
    public Task<ApiResponse<bool>> DeleteAttributeOptionAsync(string attributeId, string optionId, string token) => _apiService.DeleteAttributeOptionAsync(attributeId, optionId, token);
    public Task<ApiResponse<bool>> AssignAttributeToCategoryAsync(string categoryId, string attributeId, string token) => _apiService.AssignAttributeToCategoryAsync(categoryId, attributeId, token);
    public Task<ApiResponse<List<AttributeDefinitionModel>>> GetActiveAttributeDefinitionsAsync(string token) => _apiService.GetActiveAttributeDefinitionsAsync(token);
    public Task<ApiResponse<List<AttributeDefinitionModel>>> GetCategoryAttributesAsync(string categoryId, string token, bool includeInherited = false, bool includeInactive = false) => _apiService.GetCategoryAttributesAsync(categoryId, token, includeInherited, includeInactive);
    public Task<ApiResponse<List<AttributeOptionModel>>> GetAttributeOptionsByAttributeIdAsync(string attributeId, string token, bool onlyActive = false) => _apiService.GetAttributeOptionsByAttributeIdAsync(attributeId, token, onlyActive);
    public Task<ApiResponse<CreateProductResultModel>> CreateProductWithResultAsync(UpsertProductRequest request, string token) => _apiService.CreateProductWithResultAsync(request, token);
    public Task<ApiResponse<bool>> CreateProductAsync(UpsertProductRequest request, string token) => _apiService.CreateProductAsync(request, token);
    public Task<ApiResponse<bool>> UpdateProductAsync(string productId, UpsertProductRequest request, string token) => _apiService.UpdateProductAsync(productId, request, token);
    public Task<ApiResponse<bool>> ActivateProductAsync(string productId, string token) => _apiService.ActivateProductAsync(productId, token);
    public Task<ApiResponse<bool>> DeactivateProductAsync(string productId, string token) => _apiService.DeactivateProductAsync(productId, token);
    public Task<ApiResponse<bool>> DeleteProductAsync(string productId, string token) => _apiService.DeleteProductAsync(productId, token);
    public Task<ApiResponse<bool>> ChangeProductCategoryAsync(string productId, ChangeProductCategoryRequest request, string token) => _apiService.ChangeProductCategoryAsync(productId, request, token);
    public Task<ApiResponse<bool>> SetProductAttributeValueAsync(string productId, SetProductAttributeValueRequest request, string token) => _apiService.SetProductAttributeValueAsync(productId, request, token);
    public Task<ApiResponse<bool>> RemoveProductAttributeValueAsync(string productId, string attributeId, string token) => _apiService.RemoveProductAttributeValueAsync(productId, attributeId, token);
    public Task<ApiResponse<List<ProductAttributeValueModel>>> GetProductAttributeValuesByProductIdAsync(string productId, string token) => _apiService.GetProductAttributeValuesByProductIdAsync(productId, token);
    public Task<ApiResponse<List<ProductSummaryModel>>> SearchProductsAsync(string token, string? searchTerm = null, string? categoryId = null, int page = 1, int pageSize = 2000) => _apiService.SearchProductsAsync(token, searchTerm, categoryId, page, pageSize);
    public Task<ApiResponse<ProductDetailsModel>> GetProductDetailsWithAttributesAsync(string productId, string token) => _apiService.GetProductDetailsWithAttributesAsync(productId, token);
    public Task<ApiResponse<bool>> CreateProductVariantAsync(string productId, UpsertVariantRequest request, string token) => _apiService.CreateProductVariantAsync(productId, request, token);
    public Task<ApiResponse<bool>> UpdateProductVariantAsync(string productId, string variantId, UpsertVariantRequest request, string token) => _apiService.UpdateProductVariantAsync(productId, variantId, request, token);
    public Task<ApiResponse<bool>> ActivateProductVariantAsync(string variantId, string token) => _apiService.ActivateProductVariantAsync(variantId, token);
    public Task<ApiResponse<bool>> DeactivateProductVariantAsync(string variantId, string token) => _apiService.DeactivateProductVariantAsync(variantId, token);
    public Task<ApiResponse<bool>> DeleteProductVariantAsync(string variantId, string token) => _apiService.DeleteProductVariantAsync(variantId, token);
    public Task<ApiResponse<bool>> ChangeProductVariantTrackingPolicyAsync(string variantId, ChangeVariantTrackingPolicyRequest request, string token) => _apiService.ChangeProductVariantTrackingPolicyAsync(variantId, request, token);
    public Task<ApiResponse<bool>> ChangeProductVariantBaseUomAsync(string variantId, ChangeVariantBaseUomRequest request, string token) => _apiService.ChangeProductVariantBaseUomAsync(variantId, request, token);
    public Task<ApiResponse<bool>> LockProductVariantInventoryMovementAsync(string variantId, string token) => _apiService.LockProductVariantInventoryMovementAsync(variantId, token);
    public Task<ApiResponse<bool>> SetVariantAttributeValueAsync(string variantId, SetVariantAttributeValueRequest request, string token) => _apiService.SetVariantAttributeValueAsync(variantId, request, token);
    public Task<ApiResponse<bool>> RemoveVariantAttributeValueAsync(string variantId, string attributeId, string token) => _apiService.RemoveVariantAttributeValueAsync(variantId, attributeId, token);
    public Task<ApiResponse<List<VariantAttributeValueModel>>> GetVariantAttributeValuesByVariantIdAsync(string variantId, string token) => _apiService.GetVariantAttributeValuesByVariantIdAsync(variantId, token);
    public Task<ApiResponse<bool>> UpsertVariantUomConversionAsync(string variantId, UpsertVariantUomConversionRequest request, string token) => _apiService.UpsertVariantUomConversionAsync(variantId, request, token);
    public Task<ApiResponse<bool>> RemoveVariantUomConversionAsync(string variantId, string fromUomRef, string toUomRef, string token) => _apiService.RemoveVariantUomConversionAsync(variantId, fromUomRef, toUomRef, token);
    public Task<ApiResponse<List<ProductVariantSummaryModel>>> GetProductVariantsByProductIdAsync(string productId, string token, bool includeInactive = true) => _apiService.GetProductVariantsByProductIdAsync(productId, token, includeInactive);
    public Task<ApiResponse<ProductVariantDetailsModel>> GetProductVariantFullDetailsAsync(string variantId, string token) => _apiService.GetProductVariantFullDetailsAsync(variantId, token);
    public Task<ApiResponse<List<VariantUomConversionModel>>> GetVariantUomConversionsByVariantIdAsync(string variantId, string token) => _apiService.GetVariantUomConversionsByVariantIdAsync(variantId, token);
    public Task<ApiResponse<List<UnitOfMeasureLookupModel>>> GetUnitOfMeasureLookupAsync(string token) => _apiService.GetUnitOfMeasureLookupAsync(token);
}
