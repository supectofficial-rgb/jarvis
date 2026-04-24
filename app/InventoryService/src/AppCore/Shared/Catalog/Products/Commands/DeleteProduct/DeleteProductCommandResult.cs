namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeleteProduct;

public class DeleteProductCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
