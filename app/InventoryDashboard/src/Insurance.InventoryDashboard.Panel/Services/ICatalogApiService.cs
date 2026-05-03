using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public interface ICatalogApiService
{
    Task<ApiResponse<bool>> CreateCategoryAsync(UpsertCategoryRequest request, string token);
    Task<ApiResponse<bool>> UpdateCategoryAsync(string categoryId, UpsertCategoryRequest request, string token);
    Task<ApiResponse<bool>> MoveCategoryAsync(string categoryId, string? parentCategoryId, string token);
    Task<ApiResponse<bool>> ActivateCategoryAsync(string categoryId, string token);
    Task<ApiResponse<bool>> DeactivateCategoryAsync(string categoryId, string token);
    Task<ApiResponse<bool>> DeleteCategoryAsync(string categoryId, string token);
    Task<ApiResponse<List<CategoryNodeModel>>> GetCategoryTreeAsync(string token);
    Task<ApiResponse<List<CategoryAttributeRuleModel>>> GetCategoryAttributeRulesAsync(string categoryId, string token, bool includeInherited = true, bool includeInactive = true);
    Task<ApiResponse<bool>> AddCategoryAttributeRuleAsync(string categoryId, AddCategoryAttributeRuleRequest request, string token);
    Task<ApiResponse<bool>> UpdateCategoryAttributeRuleAsync(string categoryId, string attributeId, UpdateCategoryAttributeRuleRequest request, string token);
    Task<ApiResponse<bool>> ActivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token);
    Task<ApiResponse<bool>> DeactivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token);
    Task<ApiResponse<bool>> RemoveCategoryAttributeRuleAsync(string categoryId, string attributeId, string token);
    Task<ApiResponse<bool>> CreateAttributeDefinitionAsync(string categoryId, CreateAttributeDefinitionRequest request, string token);
    Task<ApiResponse<bool>> UpdateAttributeDefinitionAsync(string attributeId, UpdateAttributeDefinitionRequest request, string token);
    Task<ApiResponse<bool>> ActivateAttributeDefinitionAsync(string attributeId, string token);
    Task<ApiResponse<bool>> DeactivateAttributeDefinitionAsync(string attributeId, string token);
    Task<ApiResponse<bool>> DeleteAttributeDefinitionAsync(string attributeId, string token);
    Task<ApiResponse<bool>> AddAttributeOptionAsync(string attributeId, AddAttributeOptionRequest request, string token);
    Task<ApiResponse<bool>> UpdateAttributeOptionAsync(string attributeId, string optionId, UpdateAttributeOptionRequest request, string token);
    Task<ApiResponse<bool>> ActivateAttributeOptionAsync(string attributeId, string optionId, string token);
    Task<ApiResponse<bool>> DeactivateAttributeOptionAsync(string attributeId, string optionId, string token);
    Task<ApiResponse<bool>> DeleteAttributeOptionAsync(string attributeId, string optionId, string token);
    Task<ApiResponse<bool>> AssignAttributeToCategoryAsync(string categoryId, string attributeId, string token);
    Task<ApiResponse<List<AttributeDefinitionModel>>> GetActiveAttributeDefinitionsAsync(string token);
    Task<ApiResponse<List<AttributeDefinitionModel>>> GetCategoryAttributesAsync(string categoryId, string token, bool includeInherited = false, bool includeInactive = false);
    Task<ApiResponse<List<AttributeOptionModel>>> GetAttributeOptionsByAttributeIdAsync(string attributeId, string token, bool onlyActive = false);

    Task<ApiResponse<CreateProductResultModel>> CreateProductWithResultAsync(UpsertProductRequest request, string token);
    Task<ApiResponse<bool>> CreateProductAsync(UpsertProductRequest request, string token);
    Task<ApiResponse<bool>> UpdateProductAsync(string productId, UpsertProductRequest request, string token);
    Task<ApiResponse<bool>> ActivateProductAsync(string productId, string token);
    Task<ApiResponse<bool>> DeactivateProductAsync(string productId, string token);
    Task<ApiResponse<bool>> DeleteProductAsync(string productId, string token);
    Task<ApiResponse<bool>> ChangeProductCategoryAsync(string productId, ChangeProductCategoryRequest request, string token);
    Task<ApiResponse<bool>> SetProductAttributeValueAsync(string productId, SetProductAttributeValueRequest request, string token);
    Task<ApiResponse<bool>> RemoveProductAttributeValueAsync(string productId, string attributeId, string token);
    Task<ApiResponse<List<ProductAttributeValueModel>>> GetProductAttributeValuesByProductIdAsync(string productId, string token);
    Task<ApiResponse<List<ProductSummaryModel>>> SearchProductsAsync(string token, string? searchTerm = null, string? categoryId = null, int page = 1, int pageSize = 2000);
    Task<ApiResponse<ProductDetailsModel>> GetProductDetailsWithAttributesAsync(string productId, string token);

    Task<ApiResponse<bool>> CreateProductVariantAsync(string productId, UpsertVariantRequest request, string token);
    Task<ApiResponse<bool>> UpdateProductVariantAsync(string productId, string variantId, UpsertVariantRequest request, string token);
    Task<ApiResponse<bool>> ActivateProductVariantAsync(string variantId, string token);
    Task<ApiResponse<bool>> DeactivateProductVariantAsync(string variantId, string token);
    Task<ApiResponse<bool>> DeleteProductVariantAsync(string variantId, string token);
    Task<ApiResponse<bool>> ChangeProductVariantTrackingPolicyAsync(string variantId, ChangeVariantTrackingPolicyRequest request, string token);
    Task<ApiResponse<bool>> ChangeProductVariantBaseUomAsync(string variantId, ChangeVariantBaseUomRequest request, string token);
    Task<ApiResponse<bool>> LockProductVariantInventoryMovementAsync(string variantId, string token);
    Task<ApiResponse<bool>> SetVariantAttributeValueAsync(string variantId, SetVariantAttributeValueRequest request, string token);
    Task<ApiResponse<bool>> RemoveVariantAttributeValueAsync(string variantId, string attributeId, string token);
    Task<ApiResponse<List<VariantAttributeValueModel>>> GetVariantAttributeValuesByVariantIdAsync(string variantId, string token);
    Task<ApiResponse<bool>> UpsertVariantUomConversionAsync(string variantId, UpsertVariantUomConversionRequest request, string token);
    Task<ApiResponse<bool>> RemoveVariantUomConversionAsync(string variantId, string fromUomRef, string toUomRef, string token);
    Task<ApiResponse<List<ProductVariantSummaryModel>>> GetProductVariantsByProductIdAsync(string productId, string token, bool includeInactive = true);
    Task<ApiResponse<ProductVariantSearchResultModel>> SearchProductVariantsAsync(string token, string? searchTerm = null, string? productId = null, string? categoryId = null, string? attributeOptionIds = null, bool? isActive = null, int page = 1, int pageSize = 10);
    Task<ApiResponse<ProductVariantDetailsModel>> GetProductVariantFullDetailsAsync(string variantId, string token);
    Task<ApiResponse<List<VariantUomConversionModel>>> GetVariantUomConversionsByVariantIdAsync(string variantId, string token);
    Task<ApiResponse<StockDetailSearchResultModel>> SearchStockDetailsAsync(string token, string? variantId = null, bool? isEmpty = null, int page = 1, int pageSize = 200);
    Task<ApiResponse<List<WarehouseLookupItemModel>>> GetWarehouseLookupAsync(string token, bool includeInactive = true);
    Task<ApiResponse<List<SellerLookupItemModel>>> GetSellerLookupAsync(string token, bool includeInactive = true);
    Task<ApiResponse<List<LocationLookupItemModel>>> GetLocationLookupAsync(string token, string? warehouseId = null, bool includeInactive = false);
    Task<ApiResponse<List<QualityStatusLookupItemModel>>> GetQualityStatusLookupAsync(string token, bool includeInactive = false);
    Task<ApiResponse<List<InventoryTransactionListItemModel>>> GetInventoryTransactionsByVariantAsync(string variantId, string token);
    Task<ApiResponse<SellerVariantPriceSearchResultModel>> SearchSellerVariantPricesAsync(string token, Guid? sellerRef = null, Guid? variantRef = null, Guid? priceTypeRef = null, Guid? priceChannelRef = null, bool? isActive = null, int page = 1, int pageSize = 50);
    Task<ApiResponse<StockDetailBucketResultModel>> GetAvailableStockBucketsAsync(string token, Guid? variantRef = null, Guid? sellerRef = null, decimal minQuantity = 0);

    Task<ApiResponse<List<UnitOfMeasureLookupModel>>> GetUnitOfMeasureLookupAsync(string token);
}
