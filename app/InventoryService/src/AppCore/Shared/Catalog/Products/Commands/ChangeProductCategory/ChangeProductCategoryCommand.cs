namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ChangeProductCategory;

using OysterFx.AppCore.Shared.Commands;

public class ChangeProductCategoryCommand : ICommand<ChangeProductCategoryCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
}
