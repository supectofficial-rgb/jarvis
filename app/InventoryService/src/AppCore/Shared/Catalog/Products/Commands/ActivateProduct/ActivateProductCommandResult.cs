namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ActivateProduct;

public class ActivateProductCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
