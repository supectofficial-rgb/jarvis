using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public interface IApiService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password);
    Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token);

    Task<ApiResponse<List<UserSummaryModel>>> GetUsersAsync(string token);
    Task<ApiResponse<List<PersonaSummaryModel>>> GetPersonasAsync(string token);
    Task<ApiResponse<List<PermissionSummaryModel>>> GetPermissionsAsync(string token);

    Task<ApiResponse<bool>> AssignPersonaToUserAsync(string userId, string personaId, string token);
    Task<ApiResponse<bool>> RemovePersonaFromUserAsync(string userId, string personaId, string token);
    Task<ApiResponse<bool>> AssignPermissionToPersonaAsync(string personaId, string permissionId, string token);
    Task<ApiResponse<bool>> RemovePermissionFromPersonaAsync(string personaId, string permissionId, string token);

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
    Task<ApiResponse<ProductVariantDetailsModel>> GetProductVariantFullDetailsAsync(string variantId, string token);
    Task<ApiResponse<List<VariantUomConversionModel>>> GetVariantUomConversionsByVariantIdAsync(string variantId, string token);

    Task<ApiResponse<bool>> CreateWarehouseAsync(UpsertWarehouseRequest request, string token);
    Task<ApiResponse<bool>> UpdateWarehouseAsync(string warehouseId, UpsertWarehouseRequest request, string token);
    Task<ApiResponse<bool>> ActivateWarehouseAsync(string warehouseId, string token);
    Task<ApiResponse<bool>> DeactivateWarehouseAsync(string warehouseId, string token);
    Task<ApiResponse<bool>> DeleteWarehouseAsync(string warehouseId, string token);
    Task<ApiResponse<WarehouseSearchResultModel>> SearchWarehousesAsync(string token, string? code = null, string? name = null, bool? isActive = null, int page = 1, int pageSize = 20);
    Task<ApiResponse<List<WarehouseLookupItemModel>>> GetWarehouseLookupAsync(string token, bool includeInactive = true);
    Task<ApiResponse<WarehouseWithLocationsModel>> GetWarehouseWithLocationsAsync(string warehouseId, string token, bool includeInactiveLocations = true);
    Task<ApiResponse<bool>> CreateLocationAsync(UpsertLocationRequest request, string token);
    Task<ApiResponse<bool>> UpdateLocationAsync(string locationId, UpsertLocationRequest request, string token);
    Task<ApiResponse<bool>> ActivateLocationAsync(string locationId, string token);
    Task<ApiResponse<bool>> DeactivateLocationAsync(string locationId, string token);
    Task<ApiResponse<bool>> DeleteLocationAsync(string locationId, string token);
    Task<ApiResponse<bool>> MoveLocationToWarehouseAsync(string locationId, string targetWarehouseId, string token);
    Task<ApiResponse<LocationSearchResultModel>> SearchLocationsAsync(string token, string? warehouseId = null, string? locationCode = null, string? locationType = null, bool? isActive = null, int page = 1, int pageSize = 20);

    Task<ApiResponse<List<UnitOfMeasureLookupModel>>> GetUnitOfMeasureLookupAsync(string token);

    Task<ApiResponse<PriceTypeSearchResultModel>> SearchPriceTypesAsync(string token, string? code = null, string? name = null, bool? isActive = null, int page = 1, int pageSize = 50);
    Task<ApiResponse<PriceTypeLookupResultModel>> GetPriceTypeLookupAsync(string token, bool includeInactive = false);
    Task<ApiResponse<bool>> CreatePriceTypeAsync(UpsertPriceTypeRequest request, string token);
    Task<ApiResponse<bool>> UpdatePriceTypeAsync(Guid priceTypeId, UpsertPriceTypeRequest request, string token);

    Task<ApiResponse<PriceChannelSearchResultModel>> SearchPriceChannelsAsync(string token, string? code = null, string? name = null, bool? isActive = null, int page = 1, int pageSize = 50);
    Task<ApiResponse<PriceChannelLookupResultModel>> GetPriceChannelLookupAsync(string token, bool includeInactive = false);
    Task<ApiResponse<bool>> CreatePriceChannelAsync(UpsertPriceChannelRequest request, string token);
    Task<ApiResponse<bool>> UpdatePriceChannelAsync(Guid priceChannelId, UpsertPriceChannelRequest request, string token);

    Task<ApiResponse<SellerVariantPriceSearchResultModel>> SearchSellerVariantPricesAsync(string token, Guid? sellerRef = null, Guid? variantRef = null, Guid? priceTypeRef = null, Guid? priceChannelRef = null, bool? isActive = null, int page = 1, int pageSize = 50);
    Task<ApiResponse<bool>> CreateSellerVariantPriceAsync(UpsertSellerVariantPriceRequest request, string token);
    Task<ApiResponse<bool>> UpdateSellerVariantPriceAsync(Guid sellerVariantPriceId, UpsertSellerVariantPriceRequest request, string token);

    Task<ApiResponse<SellerLookupResultModel>> GetSellerLookupAsync(string token, bool includeInactive = false);
    Task<ApiResponse<StockDetailBucketResultModel>> GetAvailableStockBucketsAsync(string token, Guid? variantRef = null, Guid? sellerRef = null, decimal minQuantity = 0);
}
