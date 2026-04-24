namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ActivateProduct;

using OysterFx.AppCore.Shared.Commands;

public class ActivateProductCommand : ICommand<ActivateProductCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
}
