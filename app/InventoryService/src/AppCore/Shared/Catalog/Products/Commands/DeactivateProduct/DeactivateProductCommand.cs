namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeactivateProduct;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateProductCommand : ICommand<DeactivateProductCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
}
