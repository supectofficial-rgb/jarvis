namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IProductCommandRepository : ICommandRepository<Product, long>
{
    Task<Product?> GetByBusinessKeyAsync(Guid productBusinessKey);
    Task<bool> ExistsByBaseSkuAsync(string baseSku, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByDefaultUomRefAsync(Guid defaultUomRef, bool onlyActive = true);
    Task<bool> ExistsByCategoryRefAsync(Guid categoryRef, bool onlyActive = true);
    Task<bool> ExistsByCategorySchemaVersionRefAsync(Guid categorySchemaVersionRef, bool onlyActive = false);
    Task<bool> ExistsAttributeValueByAttributeRefAsync(Guid attributeRef, bool onlyActive = true);
    Task<bool> ExistsAttributeValueByOptionRefAsync(Guid optionRef, bool onlyActive = true);
}
