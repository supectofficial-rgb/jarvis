namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;
using OysterFx.AppCore.Shared.Queries;

public interface IProductQueryRepository : IQueryRepository
{
    Task<GetProductByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid productBusinessKey);
    Task<GetProductByBusinessKeyQueryResult?> GetByIdAsync(Guid productId);

    Task<SearchProductsQueryResult> SearchAsync(SearchProductsQuery query);
    Task<List<ProductListItem>> GetByCategoryIdAsync(Guid categoryId, bool includeInactive = false);
    Task<List<ProductListItem>> GetActiveAsync();
    Task<ProductSummaryItem?> GetSummaryAsync(Guid productId);

    Task<ProductDetailsWithAttributesItem?> GetDetailsWithAttributesAsync(Guid productId);
    Task<ProductDetailsWithVariantsItem?> GetDetailsWithVariantsAsync(Guid productId);
    Task<ProductFullDetailsItem?> GetFullDetailsAsync(Guid productId);

    Task<List<ProductAttributeValueViewItem>> GetAttributeValuesByProductIdAsync(Guid productId);
    Task<ProductAttributeValueViewItem?> GetAttributeValueByIdAsync(Guid productAttributeValueId);
    Task<List<ProductAttributeValueWithDefinitionItem>> GetAttributeValuesWithDefinitionByProductIdAsync(Guid productId);
    Task<List<MissingRequiredProductAttributeItem>> GetMissingRequiredAttributesAsync(Guid productId);

    Task<ProductCatalogFormItem?> GetCatalogFormAsync(Guid productId);
    Task<ProductCompletionStatusItem?> GetCompletionStatusAsync(Guid productId);
    Task<ProductEditorDataItem?> GetEditorDataAsync(Guid productId);
}
