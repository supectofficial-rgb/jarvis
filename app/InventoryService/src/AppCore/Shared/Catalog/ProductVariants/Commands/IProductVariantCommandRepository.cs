namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IProductVariantCommandRepository : ICommandRepository<ProductVariant, long>
{
    Task<ProductVariant?> GetByBusinessKeyAsync(Guid productVariantBusinessKey);
    Task<ProductVariant?> GetByVariantAddOnBusinessKeyAsync(Guid variantAddOnBusinessKey);
    Task<bool> DeleteVariantAddOnByBusinessKeyAsync(Guid variantAddOnBusinessKey);
    Task<ProductVariant?> GetByComponentBusinessKeyAsync(Guid variantComponentBusinessKey);
    Task<bool> DeleteVariantTagByBusinessKeyAsync(Guid variantTagBusinessKey);
    Task<bool> DeleteVariantImageByBusinessKeyAsync(Guid variantImageBusinessKey);
    Task<List<ProductVariant>> GetByProductRefAsync(Guid productRef);
    Task<bool> ExistsByVariantSkuAsync(string variantSku, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByBarcodeAsync(string barcode, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByBaseUomRefAsync(Guid baseUomRef, bool onlyActive = true);
    Task<bool> ExistsByProductRefAsync(Guid productRef, bool onlyActive = true);
    Task<bool> ExistsAttributeValueByAttributeRefAsync(Guid attributeRef, bool onlyActive = true);
    Task<bool> ExistsAttributeValueByOptionRefAsync(Guid optionRef, bool onlyActive = true);
    Task<bool> ExistsByBusinessKeyAsync(Guid productVariantBusinessKey, bool onlyActive = false);
    Task<bool> ExistsByTagRefAsync(Guid productVariantBusinessKey, Guid tagRef, Guid? exceptBusinessKey = null);
}
