namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ChangeProductCategory;

public class ChangeProductCategoryCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
}
