namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;
using OysterFx.AppCore.Shared.Queries;

public interface IProductVariantQueryRepository : IQueryRepository
{
    Task<GetProductVariantByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid productVariantBusinessKey);
    Task<GetProductVariantByBusinessKeyQueryResult?> GetByIdAsync(Guid variantId);

    Task<VariantListItem?> GetBySkuAsync(string variantSku);
    Task<VariantListItem?> GetByBarcodeAsync(string barcode);

    Task<List<VariantListItem>> GetByProductIdAsync(Guid productId, bool includeInactive = false);
    Task<SearchVariantsQueryResult> SearchAsync(SearchVariantsQuery query);
    Task<List<VariantListItem>> GetActiveAsync();
    Task<VariantSummaryItem?> GetSummaryAsync(Guid variantId);

    Task<VariantDetailsWithAttributesItem?> GetDetailsWithAttributesAsync(Guid variantId);
    Task<VariantDetailsWithProductContextItem?> GetDetailsWithProductContextAsync(Guid variantId);
    Task<VariantFullDetailsItem?> GetFullDetailsAsync(Guid variantId);

    Task<List<VariantAttributeValueViewItem>> GetAttributeValuesByVariantIdAsync(Guid variantId);
    Task<VariantAttributeValueViewItem?> GetAttributeValueByIdAsync(Guid variantAttributeValueId);
    Task<List<VariantAttributeValueWithDefinitionItem>> GetAttributeValuesWithDefinitionByVariantIdAsync(Guid variantId);
    Task<List<MissingRequiredVariantAttributeItem>> GetMissingRequiredAttributesAsync(Guid variantId);

    Task<List<VariantUomConversionViewItem>> GetUomConversionsByVariantIdAsync(Guid variantId);
    Task<VariantUomConversionViewItem?> GetUomConversionByPathAsync(Guid variantId, Guid fromUomRef, Guid toUomRef);

    Task<VariantCatalogFormItem?> GetCatalogFormAsync(Guid variantId);
    Task<VariantCompletionStatusItem?> GetCompletionStatusAsync(Guid variantId);
    Task<VariantEditorDataItem?> GetEditorDataAsync(Guid variantId);
}
