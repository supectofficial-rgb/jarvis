namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IProductVariantCommandRepository : ICommandRepository<ProductVariant, long>
{
    Task<ProductVariant?> GetByBusinessKeyAsync(Guid productVariantBusinessKey);
    Task<List<ProductVariant>> GetByProductRefAsync(Guid productRef);
    Task<bool> ExistsByVariantSkuAsync(string variantSku, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByBarcodeAsync(string barcode, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByBaseUomRefAsync(Guid baseUomRef, bool onlyActive = true);
    Task<bool> ExistsByProductRefAsync(Guid productRef, bool onlyActive = true);
    Task<bool> ExistsAttributeValueByAttributeRefAsync(Guid attributeRef, bool onlyActive = true);
    Task<bool> ExistsAttributeValueByOptionRefAsync(Guid optionRef, bool onlyActive = true);
    Task<bool> ExistsByBusinessKeyAsync(Guid productVariantBusinessKey, bool onlyActive = false);
}
