namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeactivateProduct;

public class DeactivateProductCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
