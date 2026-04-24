namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeleteProduct;

using OysterFx.AppCore.Shared.Commands;

public class DeleteProductCommand : ICommand<DeleteProductCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
}
