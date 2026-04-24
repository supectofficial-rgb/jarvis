namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.CreateProduct;

public class CreateProductCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
